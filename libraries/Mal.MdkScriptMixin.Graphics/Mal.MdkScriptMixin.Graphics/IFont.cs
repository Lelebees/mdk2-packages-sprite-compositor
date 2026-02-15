using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    public interface IFont
    {
        string Name { get; }
        float ScaleToPx(float scale);
        float PxToScale(float px);
        Vector2 MeasureText(string text, float sizePx);
        Vector2 MeasureText(StringSegment text, float sizePx);
    }
}
