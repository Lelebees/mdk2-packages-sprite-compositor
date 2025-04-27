using VRageMath;

namespace IngameScript
{
    public static class ColorExtensions
    {
        public static Color GetComplementary(this Color color)
        {
            var hsv = color.ColorToHSVDX11();
            var cH = (hsv.X + 0.5f) % 1.0f;
            var cS = hsv.Z < 0.5f ? 0.5f : hsv.Z;
            var cV = hsv.Y < 0.2f || hsv.Y > 0.8f ? 0.7f : hsv.Y;
            return new Vector3(cH, cV, cS).HSVtoColor();
        }
        
        public static Color Colorize(this Color original, Color target, float factor)
        {
            return Color.Lerp(original, original.ToGray() * target, factor);
        }
    }
}