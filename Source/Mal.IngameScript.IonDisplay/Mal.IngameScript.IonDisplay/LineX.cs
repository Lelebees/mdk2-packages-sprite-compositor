using System;
using VRageMath;

namespace IngameScript
{
    public static class LineX
    {
        public static Line Line(IIon ion, Color color, string patternId = null)
        {
            var view = ion.View<Line>();
            view.Color = color;
            view.PatternId = patternId ?? "SquareSimple";
            return view;
        }
        
        public static T RotateAround<T>(this T view, float rotation, float x, float y) where T : Line
        {
            var position = view.Start;
            var center = new Vector2(x, y);
            position -= center;
            var angle = rotation * Math.PI / 180;
            x = (float)(position.X * Math.Cos(angle) - position.Y * Math.Sin(angle));
            y = (float)(position.X * Math.Sin(angle) + position.Y * Math.Cos(angle));
            view.Start = new Vector2(x, y) + center;

            position = view.End;
            position -= center;
            x = (float)(position.X * Math.Cos(angle) - position.Y * Math.Sin(angle));
            y = (float)(position.X * Math.Sin(angle) + position.Y * Math.Cos(angle));
            view.End = new Vector2(x, y) + center;

            return view;
        }
        
        public static T Thickness<T>(this T view, float thickness) where T : Line
        {
            view.Thickness = thickness;
            return view;
        }

        public static T Between<T>(this T view, float x1, float y1, float x2, float y2) where T : Line
        {
            view.Start = new Vector2(x1, y1);
            view.End = new Vector2(x2, y2);
            return view;
        }
    }
}