using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
    public static class StackX
    {
        public static HStack HStack(this IIon ion) => ion.View<HStack>();
        public static VStack VStack(this IIon ion) => ion.View<VStack>();

        public static IReadOnlyList<HStack> Rows<T>(this IIon ion, IEnumerable<T> items, Func<T, HStack> rowFn)
        {
            var rows = new List<HStack>();
            var columnWidths = ion.Lease<List<float>>();
            try
            {
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
                    rows.Add(row);
                }

                foreach (var row in rows)
                {
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

                return rows;
            }
            finally
            {
                rows.Clear();
                columnWidths.Clear();
            }
        }
    }
}