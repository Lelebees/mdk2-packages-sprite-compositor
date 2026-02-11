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
    /// <summary>
    ///     Provides a drawing context for an <see cref="IMyTextSurface" />.
    ///     Use <see cref="Create(IMyTextSurface)" /> or
    ///     <see cref="CreateWithAspect(IMyTextSurface, AspectMode, ref RectangleF, out RectangleF)" /> to obtain an instance.
    ///     Call <see cref="BeginDraw" /> before issuing draw commands and dispose the returned <see cref="IDisposable" /> (or
    ///     call <see cref="EndDraw" />) to submit the frame.
    ///     The context applies <see cref="Transform" /> to drawing operations and supports clipping, state push/pop and sprite
    ///     capture.
    /// </summary>
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

        /// <summary>
        ///     Gets or sets the current clipping rectangle, in surface (physical) pixels.
        ///     Setting this property schedules a clip command which will be emitted before the next sprite is added.
        ///     Use this to restrict subsequent drawing to the given rectangle.
        /// </summary>
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

        /// <summary>
        ///     The transform applied to all drawing operations on this context.
        ///     Modify this to translate/scale/rotate subsequent draw calls.
        /// </summary>
        public Transform Transform { get; set; } = Transform.Identity;

        /// <summary>
        ///     Pushes the current clip and transform onto an internal stack. Dispose the returned <see cref="IDisposable" /> (or
        ///     call <see cref="Pop" />)
        ///     to restore the previous state. This is useful for scoping temporary transforms or clip rectangles.
        /// </summary>
        /// <returns>An <see cref="IDisposable" /> that will restore the previous state when disposed.</returns>
        public IDisposable Push()
        {
            _stateStack.Push(new State { ClipRect = ClipRect, Transform = Transform });
            return _stateHandle;
        }

        /// <summary>
        ///     Restores the most recently pushed clip and transform state.
        ///     Call this after <see cref="Push" /> if you did not use the returned <see cref="IDisposable" />.
        /// </summary>
        public void Pop()
        {
            var state = _stateStack.Pop();
            ClipRect = state.ClipRect;
            Transform = state.Transform;
        }

        /// <summary>
        ///     Draws a texture sprite into the specified rectangle.
        ///     The <paramref name="texture" /> parameter is the sprite texture identifier (for example a font or texture name),
        ///     <paramref name="color" /> tints the sprite, and <paramref name="rotation" /> rotates it (in radians) around the
        ///     rectangle center.
        ///     Returns the same drawing context to enable fluent calls.
        /// </summary>
        /// <param name="texture">Texture identifier to draw (e.g. texture name or font texture).</param>
        /// <param name="color">Tint color to apply to the sprite.</param>
        /// <param name="rect">Destination rectangle in virtual coordinates (transformed by <see cref="Transform" />).</param>
        /// <param name="rotation">Rotation in radians to apply around the rectangle center.</param>
        /// <returns>The same <see cref="IDc" /> instance to allow fluent chaining.</returns>
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

        /// <summary>
        ///     Fills the given rectangle using the supplied paint. The paint provides the texture and color.
        ///     Returns the same drawing context to enable fluent calls.
        /// </summary>
        /// <param name="paint">Paint describing texture and color to use.</param>
        /// <param name="rect">Destination rectangle in virtual coordinates (transformed by <see cref="Transform" />).</param>
        /// <returns>The same <see cref="IDc" /> instance to allow fluent chaining.</returns>
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

        /// <summary>
        ///     Draws a line between <paramref name="from" /> and <paramref name="to" /> with the specified thickness.
        ///     The line is drawn using the paint's texture and color. If <paramref name="capped" /> is true,
        ///     semicircular end caps are drawn using <paramref name="capTexture" />.
        ///     Coordinates are transformed by <see cref="Transform" /> before rendering.
        ///     Returns the same drawing context to enable fluent calls.
        /// </summary>
        /// <param name="paint">Paint describing texture and color for the line.</param>
        /// <param name="from">Start point of the line (virtual coordinates).</param>
        /// <param name="to">End point of the line (virtual coordinates).</param>
        /// <param name="thickness">Line thickness in virtual pixels (scaled by <see cref="Transform" />).</param>
        /// <param name="capped">If true, draw semicircular end caps using <paramref name="capTexture" />.</param>
        /// <param name="capTexture">Texture to use for end caps when <paramref name="capped" /> is true.</param>
        /// <returns>The same <see cref="IDc" /> instance to allow fluent chaining.</returns>
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

        /// <summary>
        ///     Draws text with the specified <paramref name="font" />, <paramref name="paint" />, and size in pixels (
        ///     <paramref name="sizePx" />).
        ///     <paramref name="position" /> is transformed by <see cref="Transform" />. Use <paramref name="alignment" /> to
        ///     control text alignment.
        ///     Returns the same drawing context to enable fluent calls.
        /// </summary>
        /// <param name="font">Font to use for rendering.</param>
        /// <param name="paint">Paint providing the color for the text.</param>
        /// <param name="text">Text to draw. If null or empty, no sprite will be emitted.</param>
        /// <param name="position">Position in virtual coordinates where the text baseline will be placed.</param>
        /// <param name="sizePx">Font size in virtual pixels (scaled by <see cref="Transform" />).</param>
        /// <param name="alignment">Text alignment relative to <paramref name="position" />.</param>
        /// <returns>The same <see cref="IDc" /> instance to allow fluent chaining.</returns>
        public IDc Text(IFont font, IPaint paint, string text, Vector2 position, float sizePx, TextAlignment alignment = TextAlignment.LEFT)
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
            AddSprite(ref sprite);

            return this;
        }

        /// <summary>
        ///     Measures the size, in physical pixels, of <paramref name="text" /> when drawn with <paramref name="font" /> at
        ///     <paramref name="sizePx" /> pixels.
        ///     The current <see cref="Transform" />'s scale is applied to <paramref name="sizePx" /> before measurement.
        /// </summary>
        /// <param name="font">Font used for measurement.</param>
        /// <param name="text">String to measure.</param>
        /// <param name="sizePx">Font size in virtual pixels (scaled by <see cref="Transform" />).</param>
        /// <returns>Measured size in physical pixels.</returns>
        public Vector2 MeasureText(IFont font, string text, float sizePx)
        {
            _measureBuilder.Clear();
            if (_measureBuilder.Capacity < text.Length)
                _measureBuilder.Capacity = text.Length;
            _measureBuilder.Append(text);
            var px = sizePx * Transform.Scale;
            return _surface.MeasureStringInPixels(_measureBuilder, font.Name, font.PxToScale(px));
        }

        /// <summary>
        ///     Measures the size, in physical pixels, of the <see cref="StringSegment" /> when drawn with <paramref name="font" />
        ///     at <paramref name="sizePx" /> pixels.
        ///     The current <see cref="Transform" />'s scale is applied to <paramref name="sizePx" /> before measurement.
        /// </summary>
        /// <param name="font">Font used for measurement.</param>
        /// <param name="text">Substring segment to measure.</param>
        /// <param name="sizePx">Font size in virtual pixels (scaled by <see cref="Transform" />).</param>
        /// <returns>Measured size in physical pixels.</returns>
        public Vector2 MeasureText(IFont font, StringSegment text, float sizePx)
        {
            _measureBuilder.Clear();
            if (_measureBuilder.Capacity < text.Length)
                _measureBuilder.Capacity = text.Length;
            _measureBuilder.Append(text.Text, text.Start, text.Length);
            var px = sizePx * Transform.Scale;
            return _surface.MeasureStringInPixels(_measureBuilder, font.Name, font.PxToScale(px));
        }

        /// <summary>
        ///     Adds a raw <see cref="MySprite" /> to the current draw frame and to any active capture targets.
        ///     Override this method to customize how sprites are emitted. If a clip rectangle change was scheduled,
        ///     the clip command will be emitted before the provided sprite.
        /// </summary>
        /// <param name="sprite">The sprite to emit to the current frame and any active captures.</param>
        public virtual void AddSprite(ref MySprite sprite)
        {
            if (_mustEmitClip)
            {
                _mustEmitClip = false;
                var clipSprite = CreateClipSprite();
                _frame?.Add(clipSprite);

                // Capture clip sprite if capturing
                if (_captureStack.Count > 0)
                    _captureStack.Peek().Add(clipSprite);
            }

            _frame?.Add(sprite);

            // Capture to all active capture targets
            if (_captureStack.Count > 0)
                _captureStack.Peek().Add(sprite);
        }

        /// <summary>
        ///     Adds multiple sprites to the current frame. Each sprite will be handled like a call to
        ///     <see cref="AddSprite(ref MySprite)" />.
        /// </summary>
        /// <param name="sprites">List of sprites to emit. The list contents will be iterated and emitted in order.</param>
        public void AddSprites(List<MySprite> sprites)
        {
            for (var i = 0; i < sprites.Count; i++)
            {
                var sprite = sprites[i];
                AddSprite(ref sprite);
            }
        }

        /// <summary>
        ///     Begins capturing emitted sprites into the provided <paramref name="targetList" />.
        ///     The list will be cleared and populated with sprites until <see cref="EndCapture" /> is called (or the returned
        ///     <see cref="IDisposable" /> is disposed).
        ///     Use this to record draw output for later processing or composition.
        /// </summary>
        /// <param name="targetList">List that will receive emitted sprites while capture is active.</param>
        /// <returns>An <see cref="IDisposable" /> that ends capture when disposed.</returns>
        public IDisposable BeginCapture(List<MySprite> targetList)
        {
            // Flush any pending clip change before starting capture
            if (_mustEmitClip)
            {
                var clipSprite = CreateClipSprite();
                _frame?.Add(clipSprite);
                _mustEmitClip = false;
            }

            targetList.Clear();
            _captureStack.Push(targetList);
            return _captureHandle;
        }

        /// <summary>
        ///     Ends the most recent capture session.
        ///     If a clip change was pending when capture ends, the clip command will be captured as well.
        /// </summary>
        public void EndCapture()
        {
            // Flush any pending clip change before ending capture
            if (_mustEmitClip && _captureStack.Count > 0)
            {
                var clipSprite = CreateClipSprite();
                _frame?.Add(clipSprite);
                _captureStack.Peek().Add(clipSprite); // Capture it
                _mustEmitClip = false;
            }

            _captureStack.Pop();
        }

        /// <summary>
        ///     Gets a font by name from the surface's available fonts.
        /// </summary>
        /// <param name="fontName">The font name to retrieve.</param>
        /// <returns>The requested font, or null if not found.</returns>
        public IFont GetFont(string fontName)
        {
            IFont font;
            return _fonts.TryGetValue(fontName, out font) ? font : null;
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

        /// <summary>
        ///     Creates a drawing context for the given text surface.
        /// </summary>
        /// <param name="surface">The destination text surface to draw on.</param>
        /// <returns>A configured <see cref="SurfaceDc" /> for drawing to <paramref name="surface" />.</returns>
        public static SurfaceDc Create(IMyTextSurface surface) => new SurfaceDc(surface);

        /// <summary>
        ///     Creates a drawing context for the given text surface and applies an aspect mode transformation.
        ///     The <paramref name="virtualViewport" /> parameter will be updated to the clipped virtual viewport that fits the
        ///     surface.
        /// </summary>
        /// <param name="surface">Destination <see cref="IMyTextSurface" />.</param>
        /// <param name="mode">Aspect mode used to compute the viewport transformation.</param>
        /// <param name="virtualViewport">
        ///     On input the desired virtual viewport size; on return it is adjusted to the actual
        ///     viewport.
        /// </param>
        /// <param name="physicalViewport">On return, receives the physical viewport rectangle on the surface.</param>
        /// <returns>A configured <see cref="SurfaceDc" /> with the aspect transform applied.</returns>
        /// <remarks>
        ///     The virtual viewport defines a coordinate system for drawing.
        ///     <see cref="AspectMode.Fit" /> and <see cref="AspectMode.Fill" /> preserve the virtual coordinate system but
        ///     calculate a new, centered viewport *within* that space. This new viewport represents the portion of the virtual
        ///     space that will be visible on the physical screen, adjusted for aspect ratio (letterboxed or cropped).
        ///     In contrast, the fluid modes (<see cref="AspectMode.FitAndExpand" /> and <see cref="AspectMode.FillAndContract" />)
        ///     alter the dimensions of the virtual viewport itself to match the container's aspect ratio, effectively stretching
        ///     or shrinking the coordinate system.
        ///     The returned context's <see cref="Transform" /> maps the final virtual coordinates to physical surface pixels.
        /// </remarks>
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
            if (ClipRect.Width <= 0f && ClipRect.Height <= 0f)
                return MySprite.CreateClearClipRect();

            var quantizedRect = new Rectangle(
                (int)Math.Floor(ClipRect.X),
                (int)Math.Floor(ClipRect.Y),
                (int)Math.Ceiling(ClipRect.Width),
                (int)Math.Ceiling(ClipRect.Height));
            return MySprite.CreateClipRect(quantizedRect);
        }

        /// <summary>
        ///     Begins a draw frame for the underlying surface and returns an <see cref="IDisposable" /> that will end the frame on
        ///     Dispose.
        ///     You may call <see cref="BeginDraw" /> multiple times; the underlying frame is only created on the first call and
        ///     disposed on the last <see cref="EndDraw" />.
        /// </summary>
        /// <returns>An <see cref="IDisposable" /> which will call <see cref="EndDraw" /> when disposed.</returns>
        public IDisposable BeginDraw()
        {
            if (_openCount == 0) _frame = _surface.DrawFrame();
            _openCount++;
            return _drawHandle;
        }

        /// <summary>
        ///     Ends the current draw frame. If this call balances the corresponding <see cref="BeginDraw" />, the frame will be
        ///     submitted.
        /// </summary>
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

            public Vector2 MeasureText(string text, float sizePx)
            {
                _dc._measureBuilder.Clear();
                if (_dc._measureBuilder.Capacity < text.Length)
                    _dc._measureBuilder.Capacity = text.Length;
                _dc._measureBuilder.Append(text);
                return _dc._surface.MeasureStringInPixels(_dc._measureBuilder, Name, PxToScale(sizePx));
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

            public StateHandle(SurfaceDc dc)
            {
                _dc = dc;
            }

            public void Dispose() => _dc.Pop();
        }

        class DrawHandle : IDisposable
        {
            readonly SurfaceDc _dc;

            public DrawHandle(SurfaceDc dc)
            {
                _dc = dc;
            }

            public void Dispose() => _dc.EndDraw();
        }

        class CaptureHandle : IDisposable
        {
            readonly SurfaceDc _dc;

            public CaptureHandle(SurfaceDc dc)
            {
                _dc = dc;
            }

            public void Dispose() => _dc.EndCapture();
        }
    }
}
