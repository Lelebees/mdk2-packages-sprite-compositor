using System;
using VRageMath;

namespace IngameScript
{
    public static class AspectModeExtensions
    {
        public static void CalculateViewport(this AspectMode mode, Vector2 containerSize, Vector2 virtualSize, out Transform virtualToPhysical, out RectangleF virtualViewport)
        {
            float scale;
            var scaleX = containerSize.X / virtualSize.X;
            var scaleY = containerSize.Y / virtualSize.Y;

            switch (mode)
            {
                case AspectMode.Fit:
                    scale = Math.Min(scaleX, scaleY);
                    virtualViewport = CenteredViewport(virtualSize, containerSize, scale);
                    break;

                case AspectMode.Fill:
                    scale = Math.Max(scaleX, scaleY);
                    virtualViewport = CenteredViewport(virtualSize, containerSize, scale);
                    break;

                case AspectMode.FitAndExpand:
                {
                    scale = Math.Min(scaleX, scaleY);
                    var visible = new Vector2(containerSize.X / scale, containerSize.Y / scale);
                    var expanded = virtualSize;
                    if (visible.X > virtualSize.X) expanded.X = visible.X;
                    if (visible.Y > virtualSize.Y) expanded.Y = visible.Y;
                    virtualViewport = new RectangleF(0, 0, expanded.X, expanded.Y);
                    break;
                }

                case AspectMode.FillAndContract:
                {
                    scale = Math.Max(scaleX, scaleY);
                    var visible = new Vector2(containerSize.X / scale, containerSize.Y / scale);
                    virtualViewport = new RectangleF(0, 0, visible.X, visible.Y);
                    break;
                }

                default:
                    scale = Math.Min(scaleX, scaleY);
                    virtualViewport = CenteredViewport(virtualSize, containerSize, scale);
                    break;
            }

            virtualToPhysical = new Transform(new Vector2(-virtualViewport.X * scale, -virtualViewport.Y * scale), 0f, scale);
        }

        static RectangleF CenteredViewport(Vector2 virtualSize, Vector2 containerSize, float scale)
        {
            var visible = new Vector2(containerSize.X / scale, containerSize.Y / scale);
            var off = (virtualSize - visible) * 0.5f;
            return new RectangleF(off.X, off.Y, visible.X, visible.Y);
        }
    }
}
