using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
    public static class FrameX
    {
        public static Frame Frame(this IIon ion) => ion.View<Frame>();

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
            var columnWidths = view.Context.Lease<List<float>>();
            try
            {
                var firstRow = view.Children.Count;
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
            finally
            {
                columnWidths.Clear();
            }
        }
    }
}