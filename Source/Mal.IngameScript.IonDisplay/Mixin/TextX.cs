using System;
using System.Text;
using VRageMath;

namespace IngameScript
{
    public static class TextX
    {
        static readonly StringBuilder Buf = new StringBuilder();

        public static Text Text(this IIon ion, string value, Color color)
        {
            var view = ion.View<Text>();
            view.Value = value;
            view.Color = color;
            return view;
        }

        public static Text Text(this IIon ion, Func<StringBuilder, StringBuilder> valueFn, Color color)
        {
            var view = ion.View<Text>();
            view.Value = valueFn(Buf.Clear()).ToString();
            view.Color = color;
            return view;
        }
    }
}