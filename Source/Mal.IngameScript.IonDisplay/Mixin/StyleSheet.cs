using System;
using System.Collections.Generic;

namespace IngameScript
{
    public abstract class StyleSheet
    {
        readonly Dictionary<Type, List<Style>> _root = new Dictionary<Type, List<Style>>();

        public void AddStyle(Style style)
        {
            var selector = style.Selector;
            List<Style> styles;
            if (!_root.TryGetValue(selector.Type, out styles))
            {
                styles = new List<Style>();
                _root[selector.Type] = styles;
            }

            styles.Add(style);
        }

        public void Apply(View view)
        {
            List<Style> styles;
            if (_root.TryGetValue(view.GetType(), out styles))
            {
                foreach (var style in styles)
                {
                    var selector = style.Selector;
                    if (selector.Matches(view))
                        style.Apply(view);
                }
            }
        }
    }
}