using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
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
        Vector2 MeasureString(StringSegment stringSegment, string fontId, float fontSize);
    }
}