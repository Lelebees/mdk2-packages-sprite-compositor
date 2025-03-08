using System;
using VRageMath;

namespace IngameScript
{
    public class VStack : Stack
    {
        protected override void Advance(View child, ref Vector2 position, Vector2 size)
        {
            position.Y += size.Y;
        }

        protected override void MutateSizeOnMeasure(View child, ref Vector2 size)
        {
            var childSize = child.Bounds.Size + child.Margin.Size;
            size.X = Math.Max(size.X, childSize.X);
            size.Y += childSize.Y;
        }
    }
}