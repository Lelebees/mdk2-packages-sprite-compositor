using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    public interface IIon
    {
        Theme Theme { get; set; }
        RectangleF Viewport { get; }
        IMyTextSurface Surface { get; }
        T Lease<T>() where T : class, new();
        T View<T>() where T : View, new();
        RectangleF PushClip(RectangleF dcBounds);
        RectangleF? PopClip();
    }
}