using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class Frame : View, IContainer
    {
        readonly List<View> _children = new List<View>();

        public bool ClipToBounds;

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

        protected override void OnBeforeFrame() => _children.Clear();

        protected override void OnDraw(DC dc)
        {
            if (_children == null)
                return;

            var childDc = OpenChildDc(dc);
            foreach (var child in _children)
                Draw(child, childDc);
            CloseChildDc(dc);
        }

        protected virtual void CloseChildDc(DC dc)
        {
            if (!ClipToBounds) return;
            var clip = Context.PopClip();
            if (clip.HasValue)
                dc.Add(new MySprite(SpriteType.CLIP_RECT, position: new Vector2(clip.Value.X, clip.Value.Y), size: new Vector2(clip.Value.Width, clip.Value.Height)));
            else
                dc.Add(new MySprite(SpriteType.CLIP_RECT));
        }

        protected virtual DC OpenChildDc(DC dc)
        {
            if (!ClipToBounds) return dc;
            var clip = Context.PushClip(dc.Bounds);
            dc.Add(new MySprite(SpriteType.CLIP_RECT,
                position: new Vector2(clip.X, clip.Y),
                size: new Vector2(clip.Width, clip.Height)
            ));
            return dc;
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