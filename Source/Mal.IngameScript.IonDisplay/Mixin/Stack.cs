using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public abstract class Stack : Frame
    {
        public override Vector2 Measure()
        {
            var size = Vector2.Zero;
            if (Children == null || Children.Count == 0) return size;
            foreach (var child in Children)
                MutateSizeOnMeasure(child, ref size);
            return size;
        }

        protected override void OnDraw(Action<MySprite> add, RectangleF bounds)
        {
            if (Children == null || Children.Count == 0)
                return;

            var position = bounds.Position;
            foreach (var child in Children)
            {
                var baseViewport = new RectangleF(position, child.Bounds.Size + child.Margin.Size);
                var childViewport = new RectangleF(position.X + child.Margin.Left, position.Y + child.Margin.Top, child.Bounds.Width, child.Bounds.Height);
                Draw(child, add, childViewport);
                Advance(child, ref position, baseViewport.Size);
            }
        }

        protected abstract void Advance(View child, ref Vector2 position, Vector2 size);

        protected abstract void MutateSizeOnMeasure(View child, ref Vector2 size);
    }
}