using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

// ReSharper disable PossiblyImpureMethodCallOnReadonlyVariable

namespace IngameScript
{
    public class SurfaceDc : IDc
    {
        readonly CaptureHandle _captureHandle;
        readonly Stack<List<MySprite>> _captureStack = new Stack<List<MySprite>>();
        readonly DrawHandle _drawHandle;
        readonly Dictionary<string, IFont> _fonts = new Dictionary<string, IFont>();
        readonly StringBuilder _measureBuilder = new StringBuilder();
        readonly StateHandle _stateHandle;
        readonly Stack<State> _stateStack = new Stack<State>();
        readonly IMyTextSurface _surface;
        RectangleF _clipRect;
        MySpriteDrawFrame? _frame;
        bool _mustEmitClip;
        int _openCount;

        SurfaceDc(IMyTextSurface surface)
        {
            _surface = surface;
            _stateHandle = new StateHandle(this);
            _drawHandle = new DrawHandle(this);
            _captureHandle = new CaptureHandle(this);
            InitializeFonts();
        }

        public RectangleF ClipRect
        {
            get { return _clipRect; }
            set
            {
                if (_clipRect.Equals(value)) return;
                _clipRect = value;
                _mustEmitClip = true;
            }
        }

        public Transform Transform { get; set; } = Transform.Identity;

        public IDisposable Push()
        {
            _stateStack.Push(new State { ClipRect = ClipRect, Transform = Transform });
            return _stateHandle;
        }

        public void Pop()
        {
            var state = _stateStack.Pop();
            ClipRect = state.ClipRect;
            Transform = state.Transform;
        }

        public IDc Sprite(string texture, Color color, RectangleF rect, float rotation = 0f)
        {
            if (color.A == 0) return this;
            var tRect = Transform.TransformRectCenter(rect);
            var tRotation = -Transform.Rotation + rotation;
            var sprite = new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = texture,
                Color = color,
                Position = tRect.Center,
                Size = tRect.Size,
                RotationOrScale = tRotation,
                Alignment = TextAlignment.CENTER
            };
            AddSprite(ref sprite);
            return this;
        }

        public IDc Rect(IPaint paint, RectangleF rect)
        {
            if (paint.Color.A == 0) return this;
            var tRect = Transform.TransformRectCenter(rect);
            var sprite = new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = paint.Texture,
                Color = paint.Color,
                Position = tRect.Center,
                Size = tRect.Size,
                RotationOrScale = 0f,
                Alignment = TextAlignment.CENTER
            };
            AddSprite(ref sprite);
            return this;
        }

        public IDc Line(IPaint paint, Vector2 from, Vector2 to, float thickness, bool capped = false, string capTexture = "SemiCircle")
        {
            if (paint.Color.A == 0 || thickness <= 0f) return this;
            var p0 = Transform.TransformPoint(from);
            var p1 = Transform.TransformPoint(to);
            var delta = p1 - p0;
            var length = delta.Length();
            if (length <= 0f) return this;
            var dir = delta / length;
            var angle = (float)Math.Atan2(dir.Y, dir.X);
            var tThickness = thickness * Transform.Scale;
            var center = (p0 + p1) * 0.5f;
            var sprite = new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = paint.Texture,
                Color = paint.Color,
                Position = center,
                Size = new Vector2(length, tThickness),
                RotationOrScale = angle,
                Alignment = TextAlignment.CENTER
            };
            AddSprite(ref sprite);
            if (!capped) return this;
            var capSize = new Vector2(tThickness, tThickness);
            const float halfPi = MathHelper.PiOver2;
            var startAngle = angle + halfPi;
            var endAngle = angle - halfPi;
            var startCap = new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = capTexture,
                Color = paint.Color,
                Position = p0,
                Size = capSize,
                RotationOrScale = startAngle,
                Alignment = TextAlignment.CENTER
            };
            AddSprite(ref startCap);
            var endCap = new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = capTexture,
                Color = paint.Color,
                Position = p1,
                Size = capSize,
                RotationOrScale = endAngle,
                Alignment = TextAlignment.CENTER
            };
            AddSprite(ref endCap);
            return this;
        }

        public IDc Text(IFont font, IPaint paint, string text, Vector2 position, float sizePx, TextAlignment alignment = TextAlignment.LEFT, bool measure = false)
        {
            if (string.IsNullOrEmpty(text)) return this;
            var p = Transform.TransformPoint(position);
            var px = sizePx * Transform.Scale;
            var scale = font.PxToScale(px);
            if (paint.Color.A == 0 || scale <= 0f) return this;
            var sprite = new MySprite
            {
                Type = SpriteType.TEXT,
                Data = text,
                FontId = font.Name,
                Color = paint.Color,
                Position = p,
                RotationOrScale = scale,
                Alignment = alignment
            };
            if (measure)
            {
                if (_measureBuilder.Capacity < text.Length) _measureBuilder.Capacity = text.Length;
                _measureBuilder.Clear().Append(text);
                var measuredSize = _surface.MeasureStringInPixels(_measureBuilder, font.Name, scale) / Transform.Scale;
                sprite.Size = measuredSize;
            }

            AddSprite(ref sprite);
            return this;
        }

        public void AddSprite(ref MySprite sprite)
        {
            FlushClip();
            Record(ref sprite);
        }

        public void AddSprites(IReadOnlyList<MySprite> sprites)
        {
            FlushClip();
            foreach (var s in sprites)
            {
                var sprite = s;
                Record(ref sprite);
            }
        }

        public IDisposable BeginCapture(List<MySprite> targetList)
        {
            FlushClip();
            targetList.Clear();
            _captureStack.Push(targetList);
            return _captureHandle;
        }

        public void EndCapture()
        {
            FlushClip();
            _captureStack.Pop();
        }

        public IFont GetFont(string fontName)
        {
            IFont font;
            return _fonts.TryGetValue(fontName, out font) ? font : null;
        }

        protected virtual void Record(ref MySprite sprite)
        {
            _frame?.Add(sprite);
            if (_captureStack.Count > 0) _captureStack.Peek().Add(sprite);
        }

        void FlushClip()
        {
            if (!_mustEmitClip) return;
            _mustEmitClip = false;
            var clipSprite = CreateClipSprite();
            Record(ref clipSprite);
        }

        void InitializeFonts()
        {
            var fontNames = new List<string>();
            _surface.GetFonts(fontNames);
            var m = new StringBuilder("M");
            foreach (var fontName in fontNames)
            {
                var em = _surface.MeasureStringInPixels(m, fontName, 1f).Y;
                _fonts[fontName] = new Font(this, fontName, em);
            }
        }

        public static SurfaceDc Create(IMyTextSurface surface) => new SurfaceDc(surface);

        public static SurfaceDc CreateWithAspect(IMyTextSurface surface, AspectMode mode, ref RectangleF virtualViewport, out RectangleF physicalViewport)
        {
            var dc = new SurfaceDc(surface);
            Transform virtualToPhysical;
            RectangleF cVp;
            mode.CalculateViewport(
                new Vector2(surface.SurfaceSize.X, surface.SurfaceSize.Y),
                virtualViewport.Size,
                out virtualToPhysical,
                out cVp
            );
            virtualViewport = cVp;
            physicalViewport = new RectangleF(
                (surface.TextureSize.X - surface.SurfaceSize.X) / 2f,
                (surface.TextureSize.Y - surface.SurfaceSize.Y) / 2f,
                surface.SurfaceSize.X,
                surface.SurfaceSize.Y
            );
            dc.Transform = Transform.Identity.Translate(physicalViewport.Position) * virtualToPhysical;
            return dc;
        }

        MySprite CreateClipSprite()
        {
            if (ClipRect.Width <= 0f && ClipRect.Height <= 0f) return MySprite.CreateClearClipRect();
            var quantizedRect = new Rectangle(
                (int)Math.Floor(ClipRect.X),
                (int)Math.Floor(ClipRect.Y),
                (int)Math.Ceiling(ClipRect.Width),
                (int)Math.Ceiling(ClipRect.Height));
            return MySprite.CreateClipRect(quantizedRect);
        }

        public IDisposable BeginDraw(bool render = true)
        {
            if (render && _openCount == 0) _frame = _surface.DrawFrame();
            _openCount++;
            return _drawHandle;
        }

        public void EndDraw()
        {
            _openCount--;
            if (_openCount == 0)
            {
                _frame?.Dispose();
                _frame = null;
            }
        }

        class Font : IFont
        {
            readonly SurfaceDc _dc;
            readonly float _em;

            public Font(SurfaceDc dc, string fontName, float em)
            {
                _dc = dc;
                Name = fontName;
                _em = em;
            }

            public string Name { get; }
            public float ScaleToPx(float scale) => scale * _em;
            public float PxToScale(float px) => px / _em;
            public Vector2 MeasureText(string text, float sizePx) => MeasureText(new StringSegment(text), sizePx);

            public Vector2 MeasureText(StringSegment text, float sizePx)
            {
                var mb = _dc._measureBuilder;
                var t = _dc.Transform;
                if (mb.Capacity < text.Length) mb.Capacity = text.Length;
                mb.Clear().Append(text.Text, text.Start, text.Length);
                return _dc._surface.MeasureStringInPixels(mb, Name, PxToScale(sizePx * t.Scale)) / t.Scale;
            }
        }

        struct State
        {
            public RectangleF ClipRect;
            public Transform Transform;
        }

        class StateHandle : IDisposable
        {
            readonly SurfaceDc _dc;
            public StateHandle(SurfaceDc dc) { _dc = dc; }
            public void Dispose() => _dc.Pop();
        }

        class DrawHandle : IDisposable
        {
            readonly SurfaceDc _dc;
            public DrawHandle(SurfaceDc dc) { _dc = dc; }
            public void Dispose() => _dc.EndDraw();
        }

        class CaptureHandle : IDisposable
        {
            readonly SurfaceDc _dc;
            public CaptureHandle(SurfaceDc dc) { _dc = dc; }
            public void Dispose() => _dc.EndCapture();
        }
    }
}
