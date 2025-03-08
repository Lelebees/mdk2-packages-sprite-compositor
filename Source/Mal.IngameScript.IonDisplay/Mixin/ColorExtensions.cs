using VRageMath;

namespace IngameScript
{
    public static class ColorExtensions
    {
        public static Color GetComplementary(this Color color)
        {
            // Convert to HSV.
            var hsv = color.ColorToHSVDX11();

            // Compute the complementary hue by shifting 180° (0.5 in normalized space).
            var compHue = (hsv.X + 0.5f) % 1.0f;

            // Set a minimum saturation so that even if the source is near gray,
            // the complementary will be vivid.
            var minSaturation = 0.5f;
            var compSaturation = hsv.Z < minSaturation ? minSaturation : hsv.Z;

            // For brightness (value): if the source is too dark or too bright,
            // use a mid-level brightness to ensure the complement stands out.
            var midBrightness = 0.7f;
            var compBrightness = hsv.Y < 0.2f || hsv.Y > 0.8f ? midBrightness : hsv.Y;

            // Create the complementary HSV color.
            var compHsv = new Vector3
            {
                X = compHue,
                Y = compBrightness,
                Z = compSaturation
            };

            // Convert back to RGB and return.
            return compHsv.HSVtoColor();
        }

        // {
        //     var hsv = color.ColorToHSVDX11();
        //     if (Math.Abs(hsv.Y) < 0.1f)
        //         return color;
        //     hsv.X = (hsv.X + 0.5f) % 1.0f;
        //     return hsv.HSVtoColor();
        // }
    }
}