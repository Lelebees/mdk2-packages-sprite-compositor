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

        public static Line RotateAround(this Line view, float rotation, float x, float y)
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
        
        public static Line Thickness(this Line view, float thickness)
        {
            view.Thickness = thickness;
            return view;
        }
        
        public static Line Between(this Line view, float x1, float y1, float x2, float y2)
        {
            view.Start = new Vector2(x1, y1);
            view.End = new Vector2(x2, y2);
            return view;
        }
        
        public static T RotatedAround<T>(this T view, float rotation, float x, float y, bool affectPattern = true) where T : View
        {
            var line = view as Line;
            if (line != null)
                return RotateAround(line, rotation, x, y) as T;
            
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

        public static T AutoVirtualSize<T>(this T view) where T : ViewBox
        {
            var size = view.Measure();
            view.VirtualSize = size;
            return view;
        }
        
        public static T FontSize<T>(this T view, float fontSize) where T : Text
        {
            view.FontSize = fontSize;
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
        
        public static T Add<T>(this T view, params View[] children) where T : Frame
        {
            ((IContainer)view).AddRange(children);
            return view;
        }
        
        public static T Add<T>(this T view, IEnumerable<View> children) where T : Frame
        {
            ((IContainer)view).AddRange(children);
            return view;
        }
        
        public static T Rows<T, TData>(this T view, IEnumerable<TData> items, Func<TData, HStack> rowFn) where T : Frame
        {
            var firstRow = view.Children.Count;
            var columnWidths = view.Context.Lease<List<float>>();
            foreach (var item in items)
            {
                var row = rowFn(item);
                float rowHeight = 0;
                for (var i = 0; i < row.Children.Count; i++)
                {
                    var child = row.Children[i];
                    if (child == null)
                        continue;
                    if (columnWidths.Count <= i) columnWidths.Add(0);
                    columnWidths[i] = Math.Max(columnWidths[i], child.Bounds.Width);
                    rowHeight = Math.Max(rowHeight, child.Bounds.Height + child.Margin.VerticalThickness);
                }

                row.Bounds = new RectangleF(0, 0, 0, rowHeight);
                view.Add(row);
            }

            for (var i = firstRow; i < view.Children.Count; i++)
            {
                var row = (HStack)view.Children[i];
                var width = 0f;
                for (var j = 0; j < row.Children.Count; j++)
                {
                    var child = row.Children[j];
                    if (child == null)
                        continue;
                    width += columnWidths[j] + child.Margin.HorizontalThickness;
                    child.Bounds = new RectangleF(
                        child.Bounds.X,
                        child.Bounds.Y,
                        columnWidths[j],
                        child.Bounds.Height);
                }

                row.Bounds = new RectangleF(0, 0, width, row.Bounds.Height);
            }
            
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