using System;
using VRageMath;

namespace IngameScript
{
    public class HStack : Stack
    {
        protected override void Advance(View child, ref Vector2 position, Vector2 size) => position.X += size.X;

        protected override void MutateSizeOnMeasure(View child, ref Vector2 size)
        {
            var childSize = child.Bounds.Size + child.Margin.Size;
            size.X += childSize.X;
            size.Y = Math.Max(size.Y, childSize.Y);
        }
    }
}