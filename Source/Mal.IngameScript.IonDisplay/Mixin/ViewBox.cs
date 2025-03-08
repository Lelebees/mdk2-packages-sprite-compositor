using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class ViewBox : Frame
    {
        Action<MySprite> _baseAdd;
        Vector2 _offset;
        float _scale;
        public ScaleMode ScaleMode = ScaleMode.Fit;
        public Vector2 VirtualSize;

        protected override void OnBeforeFrame()
        {
            base.OnBeforeFrame();
            VirtualSize = new Vector2(0, 0);
            ScaleMode = ScaleMode.Fit;
        }

        protected override void OnDraw(Action<MySprite> add, RectangleF bounds, RectangleF viewport)
        {
            if (Children == null)
                return;

            _baseAdd = add;
            switch (ScaleMode)
            {
                case ScaleMode.None:
                    DrawChildren(viewport);
                    break;
                case ScaleMode.Fit:
                    DrawFit(viewport);
                    break;
                case ScaleMode.Fill:
                    DrawFill(viewport);
                    break;
            }
        }

        void DrawChildren(RectangleF viewport)
        {
            if (Children.Count == 0) return;
            // First, get the extents of the children
            var extents = Children[0].Bounds;
            for (var i = 1; i < Children.Count; i++)
            {
                var child = Children[i];
                extents = new RectangleF(
                    Math.Min(extents.X, child.Bounds.X),
                    Math.Min(extents.Y, child.Bounds.Y),
                    Math.Max(extents.Right, child.Bounds.Right),
                    Math.Max(extents.Bottom, child.Bounds.Bottom));
            }

            // Then we center that into the viewport. We need to compensate for the X and Y
            // of the extent as those should not affect the final position of the children.,
            var extentSize = extents.Size;
            var extentCenter = extents.Center;
            var viewportCenter = viewport.Center;
            var extentOffset = new Vector2(viewportCenter.X - extentCenter.X, viewportCenter.Y - extentCenter.Y);
            var newViewport = new RectangleF(extentOffset.X, extentOffset.Y, extentSize.X, extentSize.Y);
            foreach (var child in Children)
                Draw(child, _baseAdd, newViewport);
        }

        void DrawFit(RectangleF viewport)
        {
            var virtualBounds = new RectangleF(0, 0, VirtualSize.X, VirtualSize.Y);
            _scale = Math.Min(viewport.Width / virtualBounds.Width, viewport.Height / virtualBounds.Height);
            var scaledBounds = new RectangleF(0, 0, virtualBounds.Width * _scale, virtualBounds.Height * _scale);
            _offset = viewport.Center - scaledBounds.Center;
            foreach (var child in Children)
                Draw(child, Add, virtualBounds);
        }

        void DrawFill(RectangleF viewport)
        {
            var virtualBounds = new RectangleF(0, 0, VirtualSize.X, VirtualSize.Y);
            _scale = Math.Max(viewport.Width / virtualBounds.Width, viewport.Height / virtualBounds.Height);
            var scaledBounds = new RectangleF(0, 0, virtualBounds.Width * _scale, virtualBounds.Height * _scale);
            _offset = viewport.Center - scaledBounds.Center;
            foreach (var child in Children)
                Draw(child, Add, virtualBounds);
        }

        void Add(MySprite sprite)
        {
            sprite.Position = sprite.Position * _scale + _offset;
            sprite.Size *= _scale;
            if (sprite.Type == SpriteType.TEXT)
                sprite.RotationOrScale *= _scale;
            _baseAdd(sprite);
        }
    }
}