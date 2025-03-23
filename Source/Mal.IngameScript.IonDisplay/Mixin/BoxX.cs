using VRageMath;

namespace IngameScript
{
    public static class BoxX
    {
        public static Box Box(this IIon ion, Color color, string patternId = null)
        {
            var box = ion.View<Box>();
            box.Color = color;
            box.PatternId = patternId;
            return box;
        }
        
        public static T RotatedImg<T>(this T view, float rotation) where T : Box
        {
            view.Rotation = rotation;
            return view;
        }

        public static T MirroredImg<T>(this T view) where T : Box
        {
            view.Mirror = true;
            return view;
        }

        public static T FlippedImg<T>(this T view) where T : Box
        {
            view.Flip = true;
            return view;
        }
    }
}