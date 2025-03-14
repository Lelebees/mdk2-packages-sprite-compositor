using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    public interface IContext
    {
        IMyTextSurface Surface { get; }
        T Lease<T>() where T : class, new();
        RectangleF PushClip(RectangleF dcBounds);
        RectangleF? PopClip();
    }
}