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

        protected override void OnDraw(DC dc)
        {
            if (Children == null || Children.Count == 0)
                return;

            var childDc = OpenChildDc(dc);

            var position = childDc.Bounds.Position;
            foreach (var child in Children)
            {
                var baseBounds = new RectangleF(position, child.Bounds.Size + child.Margin.Size);
                var childBounds = new RectangleF(position.X + child.Margin.Left, position.Y + child.Margin.Top, child.Bounds.Width, child.Bounds.Height);
                Draw(child, childDc.WithBounds(childBounds));
                Advance(child, ref position, baseBounds.Size);
            }

            CloseChildDc(childDc);
        }

        protected abstract void Advance(View child, ref Vector2 position, Vector2 size);

        protected abstract void MutateSizeOnMeasure(View child, ref Vector2 size);
    }
}