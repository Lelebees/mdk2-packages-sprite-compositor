using VRageMath;

namespace IngameScript
{
    public static class ViewBoxX
    {
        public static ViewBox ViewBox(this IIon ion, float virtualWidth, float virtualHeight)
        {
            var view = ion.View<ViewBox>();
            view.VirtualSize = new Vector2(virtualWidth, virtualHeight);
            return view;
        }

        public static T AutoVirtualSize<T>(this T view) where T : ViewBox
        {
            var size = view.Measure();
            view.VirtualSize = size;
            return view;
        }

        public static T Unscaled<T>(this T view) where T : ViewBox
        {
            view.ScaleMode = ScaleMode.None;
            return view;
        }

        public static T ScaledToFit<T>(this T view) where T : ViewBox
        {
            view.ScaleMode = ScaleMode.Fit;
            return view;
        }

        public static T ScaledToFill<T>(this T view) where T : ViewBox
        {
            view.ScaleMode = ScaleMode.Fill;
            return view;
        }

        public static T Padding<T>(this T view, float left, float top, float right, float bottom) where T : ViewBox
        {
            view.Padding = new Thickness(left, top, right, bottom);
            return view;
        }

        public static T Padding<T>(this T view, float uniformSize) where T : ViewBox
        {
            view.Padding = new Thickness(uniformSize);
            return view;
        }

        public static T Padding<T>(this T view, float horizontal, float vertical) where T : ViewBox
        {
            view.Padding = new Thickness(horizontal, vertical);
            return view;
        }

        public static T Padding<T>(this T view, Thickness padding) where T : ViewBox
        {
            view.Padding = padding;
            return view;
        }
    }
}