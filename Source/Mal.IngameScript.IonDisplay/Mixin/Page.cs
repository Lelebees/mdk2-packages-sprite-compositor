using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public abstract class Page { }

    public abstract class Page<TModel> : Page
    {
        readonly Action<MySprite> _add;
        readonly Context _context;
        readonly Dictionary<Type, ICache> _viewCache = new Dictionary<Type, ICache>();
        MySpriteDrawFrame _frame;
        bool _inject;
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
            _context.Begin(surface, _viewport);
            ForceSync();
        }

        void ForceSync()
        {
            _inject = !_inject;
            if (_inject)
                _frame.Add(new MySprite());
        }

        void EndFrame()
        {
            _context.End();
            _frame.Dispose();
            foreach (var cache in _viewCache.Values)
                cache.Reset();
        }

        public void Render(TModel model, IMyTextSurface surface, bool force = false)
        {
            BeginFrame(surface);
            if (force)
                ForceSync();
            IView view = Render(_context, model);
            var dc = new DC(_add, _viewport);
            view.Draw(dc);
            EndFrame();
        }

        void Add(MySprite sprite) => _frame.Add(sprite);

        protected abstract View Render(IIon ion, TModel model);

        interface ICache
        {
            object Lease();
            void Reset();
        }

        class Cache<T> : ICache where T : class, new()
        {
            readonly List<T> _items = new List<T>();
            int _index;

            public object Lease()
            {
                T view;
                if (_index < _items.Count)
                    view = _items[_index];
                else
                    _items.Add(view = new T());
                _index++;
                return view;
            }

            public void Reset() => _index = 0;
        }

        class Context : IIon
        {
            readonly Stack<RectangleF> _clipStack = new Stack<RectangleF>();
            readonly Page<TModel> _page;

            public Context(Page<TModel> page)
            {
                _page = page;
            }

            public IMyTextSurface Surface { get; private set; }
            public T Lease<T>() where T : class, new() => _page.Lease<T>();
            public T View<T>() where T : View, new() => _page.PullView<T>();

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

            public void Begin(IMyTextSurface surface, RectangleF viewport)
            {
                Surface = surface;
                Viewport = viewport;
                Theme.UpdateFrom(surface);
            }

            public Theme Theme { get; set; } = new Theme();
            public RectangleF Viewport { get; private set; }
            public void End() => _clipStack.Clear();
        }
    }
}