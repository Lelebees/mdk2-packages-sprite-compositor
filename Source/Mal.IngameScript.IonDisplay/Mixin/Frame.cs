using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
    public class Frame : View, IContainer
    {
        readonly List<View> _children = new List<View>();
        readonly Action<RectangleF?> _clip;
        DC _dc;

        public bool Clip;

        public Frame()
        {
            _clip = ClipFn;
        }

        public IReadOnlyList<View> Children => _children;

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

        protected override void OnBeforeFrame()
        {
            _children.Clear();
        }

        void ClipFn(RectangleF? clip)
        {
            if (clip == null)
            {
                _dc.Clip(_dc.Bounds);
                return;
            }

            RectangleF a = _dc.Bounds, b = clip.Value, c;
            RectangleF.Intersect(ref a, ref b, out c);
            _dc.Clip(c);
        }

        protected override void OnDraw(DC dc)
        {
            if (_children == null)
                return;

            var childDc = OpenChildDc(dc);

            foreach (var child in _children)
                Draw(child, childDc);

            CloseChildDc();
        }

        protected void CloseChildDc()
        {
            if (Clip) _dc.Clip(null);
            _dc = default(DC);
        }

        protected DC OpenChildDc(DC dc)
        {
            if (!Clip) return dc;
            _dc = dc;
            _dc.Clip(dc.Bounds);
            return dc.WithClip(_clip);
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
    }
}