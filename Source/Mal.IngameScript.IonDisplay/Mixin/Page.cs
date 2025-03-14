using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public abstract class Page
    {
        //public abstract void Render(object model, IMyTextSurface surface);
    }

    public abstract class Page<TModel> : Page
    {
        readonly Action<MySprite> _add;
        readonly StringBuilder _buffer = new StringBuilder();
        readonly Context _context;
        readonly Dictionary<Type, ICache> _viewCache = new Dictionary<Type, ICache>();
        MySpriteDrawFrame _frame;

        bool _inject;
        long _tickCount;
        RectangleF _viewport;

        protected Page()
        {
            _context = new Context(this);
            _add = Add;
        }

        T Lease<T>() where T : class, new()
        {
            ICache cache;
            if (!_viewCache.TryGetValue(typeof(T), out cache))
                _viewCache[typeof(T)] = cache = new Cache<T>();
            return (T)cache.Lease();
        }

        T PullView<T>() where T : View, new()
        {
            var view = Lease<T>();
            ((IView)view).BeginFrame(_context);
            return view;
        }

        void BeginFrame(IMyTextSurface surface)
        {
            var surfaceSize = surface.SurfaceSize;
            var textureSize = surface.TextureSize;
            _viewport = new RectangleF(
                (textureSize - surfaceSize) / 2,
                surfaceSize);
            _frame = surface.DrawFrame();
            // Workaround: Every 5 seconds (300 frames), toggle the injection flag to force a resynchronization
            // with the server.
            _context.Begin(surface);
            if (_tickCount % 300 == 0)
                _inject = !_inject;
            if (_inject)
                _frame.Add(new MySprite());
            _tickCount++;
        }

        void EndFrame()
        {
            _context.End();
            _frame.Dispose();
            foreach (var cache in _viewCache.Values)
                cache.Reset();
        }

        public Frame Frame()
        {
            return PullView<Frame>();
        }

        public ViewBox ViewBox(float virtualWidth, float virtualHeight)
        {
            var view = PullView<ViewBox>();
            view.VirtualSize = new Vector2(virtualWidth, virtualHeight);
            return view;
        }

        public Box Box(Color color, string patternId = null)
        {
            var view = PullView<Box>();
            view.Page = this;
            view.Color = color;
            view.PatternId = patternId ?? CommonPatterns.Square;
            return view;
        }

        public Line Line(Color color, string patternId = null)
        {
            var view = PullView<Line>();
            view.Color = color;
            view.PatternId = patternId ?? CommonPatterns.Square;
            return view;
        }

        public VStack VStack()
        {
            return PullView<VStack>();
        }

        public IReadOnlyList<HStack> Rows<T>(IEnumerable<T> items, Func<T, HStack> rowFn)
        {
            var rows = new List<HStack>();
            var columnWidths = new List<float>();
            foreach (var item in items)
            {
                var row = rowFn(item);
                float rowHeight = 0;
                for (var i = 0; i < row.Children.Count; i++)
                {
                    var child = row.Children[i];
                    if (child == null)
                        continue;
                    if (columnWidths.Count <= i) columnWidths.Add(0);
                    columnWidths[i] = Math.Max(columnWidths[i], child.Bounds.Width);
                    rowHeight = Math.Max(rowHeight, child.Bounds.Height + child.Margin.VerticalThickness);
                }

                row.Bounds = new RectangleF(0, 0, 0, rowHeight);
                rows.Add(row);
            }

            foreach (var row in rows)
            {
                var width = 0f;
                for (var j = 0; j < row.Children.Count; j++)
                {
                    var child = row.Children[j];
                    if (child == null)
                        continue;
                    width += columnWidths[j] + child.Margin.HorizontalThickness;
                    child.Bounds = new RectangleF(
                        child.Bounds.X,
                        child.Bounds.Y,
                        columnWidths[j],
                        child.Bounds.Height);
                }

                row.Bounds = new RectangleF(0, 0, width, row.Bounds.Height);
            }

            return rows;
        }

        public HStack HStack()
        {
            return PullView<HStack>();
        }

        public Text Text(string value, Color color)
        {
            var view = PullView<Text>();
            view.Value = value;
            view.Color = color;
            return view;
        }

        public Text Text(Func<StringBuilder, StringBuilder> valueFn, Color color)
        {
            var view = PullView<Text>();
            view.Value = valueFn(_buffer.Clear()).ToString();
            view.Color = color;
            return view;
        }

        // public sealed override void Render(object model, IMyTextSurface surface)
        //     => Render((TModel)model, surface);

        public void Render(TModel model, IMyTextSurface surface)
        {
            BeginFrame(surface);
            var view = Render(surface, model, _viewport);
            var dc = new DC(_add, _viewport);
            ((IView)view).Draw(dc);
            EndFrame();
        }

        void Add(MySprite sprite)
        {
            _frame.Add(sprite);
        }
        
        protected abstract View Render(IMyTextSurface surface, TModel model, RectangleF viewport);

        interface ICache
        {
            object Lease();
            void Reset();
        }

        class Cache<T> : ICache where T : class, new()
        {
            readonly List<T> _views = new List<T>();
            int _index;

            public object Lease()
            {
                T view;
                if (_index < _views.Count)
                    view = _views[_index];
                else
                    _views.Add(view = new T());

                _index++;
                return view;
            }

            public void Reset()
            {
                _index = 0;
            }
        }

        class Context : IContext
        {
            readonly Stack<RectangleF> _clipStack = new Stack<RectangleF>();
            readonly Page<TModel> _page;

            public Context(Page<TModel> page)
            {
                _page = page;
            }

            public IMyTextSurface Surface { get; private set; }

            public T Lease<T>() where T : class, new()
            {
                return _page.Lease<T>();
            }

            public RectangleF PushClip(RectangleF bounds)
            {
                if (_clipStack.Count == 0)
                {
                    _clipStack.Push(bounds);
                    return bounds;
                }

                RectangleF a = _clipStack.Peek(), c;
                RectangleF.Intersect(ref a, ref bounds, out c);
                _clipStack.Push(c);
                return c;
            }

            public RectangleF? PopClip()
            {
                _clipStack.Pop();
                return _clipStack.Count > 0 ? _clipStack.Peek() : (RectangleF?)null;
            }

            public void Begin(IMyTextSurface surface)
            {
                Surface = surface;
            }

            public void End()
            {
                _clipStack.Clear();
            }
        }

        protected static class CommonPatterns
        {
            public const string Circle = "Circle";
            public const string SemiCircle = "SemiCircle";
            public const string CircleHollow = "CircleHollow";
            public const string Square = "SquareSimple";
            public const string SquareHollow = "SquareHollow";
            public const string Triangle = "Triangle";
            public const string RightTriangle = "RightTriangle";

            public const string BusyIndicator = "Screen_LoadingBar";
            public const string BracketL = "DecorativeBracketLeft";
            public const string BracketR = "DecorativeBracketRight";
            public const string Grid = "Grid";
        }
    }
}