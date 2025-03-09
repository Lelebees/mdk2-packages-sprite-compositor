using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public abstract class View : IView
    {
        public RectangleF Bounds;
        public IContext Context;
        public bool IsVisible = true;
        public Thickness Margin;
        public Flexing Flex;
        public readonly HashSet<string> Classes = new HashSet<string>(StringComparer.Ordinal);
        public View Parent;

        void IView.BeginFrame(IContext context)
        {
            Bounds = new RectangleF(0, 0, -1, -1);
            Context = context;
            IsVisible = true;
            Margin = new Thickness(0);
            Flex = Flexing.None;
            Parent = null;
            Classes.Clear();
            OnBeforeFrame();
        }

        protected abstract void OnBeforeFrame();

        public Vector2 Measure(bool withMargins)
        {
            var size = Measure();
            if (withMargins)
                size += Margin.Size;
            return size;
        }
        
        public virtual Vector2 Measure()
        {
            return Vector2.Zero;
        }
        
        void IView.Draw(Action<MySprite> add, RectangleF viewport)
        {
            var bounds = Bounds;
            bounds = new RectangleF(
                viewport.X + bounds.X,
                viewport.Y + bounds.Y,
                bounds.Width < 0 ? viewport.Width : bounds.Width,
                bounds.Height < 0 ? viewport.Height : bounds.Height);
            OnDraw(add, bounds);
        }

        protected static void Draw(View view, Action<MySprite> add, RectangleF viewport)
        {
            if (view.IsVisible)
                ((IView)view).Draw(add, viewport);
        }

        protected abstract void OnDraw(Action<MySprite> add, RectangleF bounds);
    }
}