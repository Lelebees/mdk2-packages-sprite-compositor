using System;
using System.Collections.Immutable;
using VRageMath;

namespace IngameScript
{
    public abstract class Style
    {
        Style(IStyleSelector selector)
        {
            Selector = selector;
        }

        public IStyleSelector Selector { get; }

        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public string PatternId { get; set; }
        public string FontId { get; set; }
        public float FontSize { get; set; }
        public bool IsVisible { get; set; }
        public float Opacity { get; set; }

        public abstract void Apply(View view);

        public static SelectorBuilder<T, T> For<T>() where T : View => new SelectorBuilder<T, T>(0, null, ImmutableArray<string>.Empty);

        class StyleImpl<T> : Style where T : View
        {
            readonly ImmutableArray<Action<T>> _actions;

            public StyleImpl(IStyleSelector selector, ImmutableArray<Action<T>> actions) : base(selector)
            {
                _actions = actions;
            }

            public override void Apply(View view)
            {
                foreach (var action in _actions)
                    action((T)view);
            }
        }

        class StyleSelector<T> : IStyleSelector where T : View
        {
            protected readonly ImmutableArray<string> Classes;

            public StyleSelector(IStyleSelector parent, ImmutableArray<string> classes)
            {
                Parent = parent;
                Classes = classes;
            }

            public Type Type => typeof(T);
            public IStyleSelector Parent { get; }

            public virtual bool Matches(View view) => view is T && view.Classes.ContainsAll(Classes) && (Parent == null || Parent.Matches(view));
        }

        class DescendantStyleSelector<T> : StyleSelector<T> where T : View
        {
            public DescendantStyleSelector(IStyleSelector parent, ImmutableArray<string> classes) : base(parent, classes) { }

            public override bool Matches(View view)
            {
                while (view != null)
                {
                    if (view is T && view.Classes.ContainsAll(Classes))
                        return true;
                    view = view.Parent;
                }

                return false;
            }
        }

        class ChildStyleSelector<T> : StyleSelector<T> where T : View
        {
            public ChildStyleSelector(IStyleSelector parent, ImmutableArray<string> classes) : base(parent, classes) { }

            public override bool Matches(View view)
            {
                var parent = view.Parent;
                return base.Matches(parent);
            }
        }

        public struct SelectorBuilder<T, T2> where T : View where T2 : View
        {
            readonly int _type;
            readonly IStyleSelector _styleSelector;
            ImmutableArray<string> _classes;

            public SelectorBuilder(int type, IStyleSelector styleSelector, ImmutableArray<string> classes)
            {
                _type = type;
                _styleSelector = styleSelector;
                _classes = classes;
            }

            IStyleSelector FinalizeSelector()
            {
                switch (_type)
                {
                    case 1:
                        return new DescendantStyleSelector<T2>(_styleSelector, _classes);
                    case 2:
                        return new ChildStyleSelector<T2>(_styleSelector, _classes);
                    default:
                        return new StyleSelector<T2>(_styleSelector, _classes);
                }
            }

            public SelectorBuilder<T, TAncestor> DescendantOf<TAncestor>() where TAncestor : View => new SelectorBuilder<T, TAncestor>(1, FinalizeSelector(), ImmutableArray<string>.Empty);

            public SelectorBuilder<T, View> DescendantOf(string className) => DescendantOf<View>().WithClass(className);

            public SelectorBuilder<T, TParent> ChildOf<TParent>() where TParent : View => new SelectorBuilder<T, TParent>(2, FinalizeSelector(), ImmutableArray<string>.Empty);

            public SelectorBuilder<T, View> ChildOf(string className) => ChildOf<View>().WithClass(className);

            public SelectorBuilder<T, T2> WithClass(string className) => new SelectorBuilder<T, T2>(_type, _styleSelector, _classes.Add(className));

            public StyleBuilder<T> Style() => new StyleBuilder<T>(FinalizeSelector(), ImmutableArray<Action<T>>.Empty);

            public StyleBuilder<T> Apply(Action<T> action) => new StyleBuilder<T>(FinalizeSelector(), ImmutableArray.Create(action));
        }

        public struct StyleBuilder<T> where T : View
        {
            readonly IStyleSelector _styleSelector;
            readonly ImmutableArray<Action<T>> _actions;

            public StyleBuilder(IStyleSelector styleSelector, ImmutableArray<Action<T>> actions)
            {
                _styleSelector = styleSelector;
                _actions = actions;
            }

            public StyleBuilder<T> And(Action<T> action) => new StyleBuilder<T>(_styleSelector, _actions.Add(action));

            public Style RegisterIn(StyleSheet styleSheet)
            {
                var style = new StyleImpl<T>(_styleSelector, _actions);
                styleSheet.AddStyle(style);
                return style;
            }
        }
    }
}