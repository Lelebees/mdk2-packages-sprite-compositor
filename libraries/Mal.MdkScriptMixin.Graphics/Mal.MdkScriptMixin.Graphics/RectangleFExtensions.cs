using VRageMath;

namespace IngameScript
{
    public static class RectangleFExtensions
    {
        public static RectangleF CenteredIn(this RectangleF rect, RectangleF container) =>
            new RectangleF(
                container.X + (container.Width - rect.Width) / 2f,
                container.Y + (container.Height - rect.Height) / 2f,
                rect.Width,
                rect.Height
            );

        public static RectangleF ClipTo(this RectangleF rect, RectangleF clipRect)
        {
            var me = rect;
            RectangleF result;
            RectangleF.Intersect(ref me, ref clipRect, out result);
            return result;
        }
    }
}
