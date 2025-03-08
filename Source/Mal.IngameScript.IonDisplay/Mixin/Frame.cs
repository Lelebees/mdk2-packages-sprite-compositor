using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class Frame : View, IContainer
    {
        readonly List<View> _children = new List<View>();
        
        public IReadOnlyList<View> Children => _children;

        protected override void OnBeforeFrame() => _children.Clear();

        protected override void OnDraw(Action<MySprite> add, RectangleF bounds, RectangleF viewport)
        {
            if (_children == null)
                return;

            foreach (var child in _children)
                Draw(child, add, bounds);
        }

        public override Vector2 Measure()
        {
            if (_children == null || _children.Count == 0)
                return Vector2.Zero;

            var extents = _children[0].Bounds;
            for (var i = 1; i < _children.Count; i++)
            {
                var child = _children[i];
                extents = new RectangleF(
                    Math.Min(extents.X, child.Bounds.X),
                    Math.Min(extents.Y, child.Bounds.Y),
                    Math.Max(extents.Right, child.Bounds.Right),
                    Math.Max(extents.Bottom, child.Bounds.Bottom));
            }

            return extents.Size;
        }

        void IContainer.Add(View view)
        {
            _children.Add(view);
            view.Parent = this;
        }

        void IContainer.AddRange(IEnumerable<View> views)
        {
            foreach (var view in views)
            {
                _children.Add(view);
                view.Parent = this;
            }
        }
    }
}