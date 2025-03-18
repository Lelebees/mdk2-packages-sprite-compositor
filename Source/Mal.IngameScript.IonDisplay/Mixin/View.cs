using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
    public abstract class View : IView
    {
        public readonly HashSet<string> Classes = new HashSet<string>(StringComparer.Ordinal);
        public RectangleF Bounds;
        public IContext Context;
        public Flexing Flex;
        public bool IsVisible = true;
        public Thickness Margin;
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

        void IView.Draw(DC dc)
        {
            var bounds = Bounds;
            bounds = new RectangleF(
                dc.Bounds.X + bounds.X,
                dc.Bounds.Y + bounds.Y,
                bounds.Width < 0 ? dc.Bounds.Width : bounds.Width,
                bounds.Height < 0 ? dc.Bounds.Height : bounds.Height);
            OnDraw(dc.WithBounds(bounds));
        }

        protected abstract void OnBeforeFrame();

        public Vector2 Measure(bool withMargins)
        {
            var size = Measure();
            if (withMargins)
                size += Margin.Size;
            return size;
        }

        public virtual Vector2 Measure() => Vector2.Zero;

        protected static void Draw(View view, DC dc)
        {
            if (view.IsVisible)
                ((IView)view).Draw(dc);
        }

        protected abstract void OnDraw(DC dc);
    }
}