using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public static class ViewExtensions
    {
        public static T CenteredAt<T>(this T view, float x, float y) where T : View
            => CenteredAt(view, new Vector2(x, y));

        public static T CenteredAt<T>(this T view, Vector2 position) where T : View
        {
            view.Bounds = new RectangleF(
                position - view.Bounds.Size / 2,
                view.Bounds.Size);
            return view;
        }

        public static T At<T>(this T view, float x, float y) where T : View
            => At(view, new Vector2(x, y));

        public static T At<T>(this T view, Vector2 position) where T : View
        {
            view.Bounds = new RectangleF(
                position,
                view.Bounds.Size);
            return view;
        }

        public static T CenterVAt<T>(this T view, float x, float y) where T : View
        {
            view.Bounds = new RectangleF(
                new Vector2(x, y - view.Bounds.Height / 2),
                view.Bounds.Size);
            return view;
        }

        public static T CenterHAt<T>(this T view, float x, float y) where T : View
        {
            view.Bounds = new RectangleF(
                new Vector2(x - view.Bounds.Width / 2, y),
                view.Bounds.Size);
            return view;
        }

        public static T Size<T>(this T view, float width, float height) where T : View
        {
            view.Bounds = new RectangleF(view.Bounds.Position, new Vector2(width, height));
            return view;
        }

        public static T AutoSize<T>(this T view, float fixedWidth = -1f, float fixedHeight = -1f) where T : View
        {
            var size = view.Measure();
            if (fixedWidth >= 0) size.X = fixedWidth;
            if (fixedHeight >= 0) size.Y = fixedHeight;
            view.Bounds = new RectangleF(view.Bounds.Position, size);
            return view;
        }
        
        public static T RotateAround<T>(this T view, float rotation, float x, float y, bool affectPattern = true) where T : View
        {
            var line = view as Line;
            if (line != null)
                return line.RotateAround(rotation, x, y) as T;

            var position = view.Bounds.Position;
            var center = new Vector2(x, y);
            position -= center;
            var angle = MathHelper.ToRadians(rotation);
            x = (float)(position.X * Math.Cos(angle) - position.Y * Math.Sin(angle));
            y = (float)(position.X * Math.Sin(angle) + position.Y * Math.Cos(angle));
            position = new Vector2(x, y);
            position += center;
            view.Bounds = new RectangleF(position, view.Bounds.Size);
            if (affectPattern)
            {
                var box = view as Box;
                if (box != null)
                    box.Rotation = (box.Rotation + rotation) % 360;
            }

            return view;
        }


        public static T FontSize<T>(this T view, float fontSize) where T : Text
        {
            view.FontSize = fontSize;
            return view;
        }
        
        public static T AlignRight<T>(this T view) where T : Text
        {
            view.Alignment = TextAlignment.RIGHT;
            return view;
        }

        public static T AlignCenter<T>(this T view) where T : Text
        {
            view.Alignment = TextAlignment.CENTER;
            return view;
        }

        public static T Hidden<T>(this T view) where T : View
        {
            view.IsVisible = false;
            return view;
        }

        public static T Visibility<T>(this T view, bool state) where T : View
        {
            view.IsVisible = state;
            return view;
        }

        public static T Margin<T>(this T view, float left, float top, float right, float bottom) where T : View
        {
            view.Margin = new Thickness(left, top, right, bottom);
            return view;
        }

        public static T Margin<T>(this T view, float uniformSize) where T : View
        {
            view.Margin = new Thickness(uniformSize);
            return view;
        }

        public static T Margin<T>(this T view, float horizontal, float vertical) where T : View
        {
            view.Margin = new Thickness(horizontal, vertical);
            return view;
        }

        public static T Margin<T>(this T view, Thickness margin) where T : View
        {
            view.Margin = margin;
            return view;
        }
        
        public static T Flex<T>(this T view) where T : View
        {
            view.Flex = Flexing.Width | Flexing.Height;
            return view;
        }

        public static T FlexWidth<T>(this T view) where T : View
        {
            view.Flex = Flexing.Width;
            return view;
        }

        public static T FlexHeight<T>(this T view) where T : View
        {
            view.Flex = Flexing.Height;
            return view;
        }

        public static T Class<T>(this T view, string className) where T : View
        {
            view.Classes.Add(className);
            return view;
        }

        public static T Clipped<T>(this T view) where T : Frame
        {
            view.ClipToBounds = true;
            return view;
        }
    }
}