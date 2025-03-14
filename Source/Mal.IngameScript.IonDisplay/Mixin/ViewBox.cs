using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class ViewBox : Frame
    {
        Action<MySprite> /*_baseAdd, */_viewAdd;
        Vector2 _offset;
        float _scale;
        public Thickness Padding;
        public ScaleMode ScaleMode = ScaleMode.Fit;
        public Vector2 VirtualSize;
        DC _baseDc;

        protected override void OnBeforeFrame()
        {
            base.OnBeforeFrame();
            VirtualSize = new Vector2(0, 0);
            ScaleMode = ScaleMode.Fit;
            _viewAdd = Add;
        }

        protected sealed override void OnDraw(DC dc)
        {
            if (Children == null)
                return;
            var childDc = OpenChildDc(dc);
            DrawContent(childDc);
            CloseChildDc(childDc);
        }

        protected override DC OpenChildDc(DC dc)
        {
            _baseDc = dc;
            return base.OpenChildDc(dc.WithAdd(_viewAdd));
        }

        protected override void CloseChildDc(DC childDc)
        {
            base.CloseChildDc(_baseDc);
            _baseDc = default(DC);
        }

        protected virtual void DrawContent(DC childDc)
        {
            switch (ScaleMode)
            {
                case ScaleMode.None:
                    DrawChildren(childDc);
                    break;
                case ScaleMode.Fit:
                    DrawFit(childDc);
                    break;
                case ScaleMode.Fill:
                    DrawFill(childDc);
                    break;
            }
        }

        void DrawChildren(DC dc)
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
            var viewportCenter = dc.Bounds.Center;
            var extentOffset = new Vector2(viewportCenter.X - extentCenter.X, viewportCenter.Y - extentCenter.Y);
            var newBounds = new RectangleF(extentOffset.X, extentOffset.Y, extentSize.X, extentSize.Y);

            // Padding is in the virtual space. 
            newBounds = new RectangleF(
                newBounds.X,
                newBounds.Y,
                newBounds.Width,
                newBounds.Height);

            foreach (var child in Children)
                Draw(child, dc.WithBounds(newBounds));
        }

        void DrawFit(DC dc)
        {
            var virtualBounds = new RectangleF(0, 0, VirtualSize.X + Padding.HorizontalThickness, VirtualSize.Y + Padding.VerticalThickness);
            _scale = Math.Min(dc.Bounds.Width / virtualBounds.Width, dc.Bounds.Height / virtualBounds.Height);
            var scaledBounds = new RectangleF(Padding.Left, Padding.Top, virtualBounds.Width * _scale, virtualBounds.Height * _scale);
            _offset = dc.Bounds.Center - scaledBounds.Center;
            virtualBounds = new RectangleF(Padding.Left, Padding.Top, VirtualSize.X, VirtualSize.Y);
            foreach (var child in Children)
                Draw(child, dc.WithBounds(virtualBounds).WithAdd(_viewAdd));
        }

        void DrawFill(DC dc)
        {
            var virtualBounds = new RectangleF(0, 0, VirtualSize.X + Padding.HorizontalThickness, VirtualSize.Y + Padding.VerticalThickness);
            _scale = Math.Max(dc.Bounds.Width / virtualBounds.Width, dc.Bounds.Height / virtualBounds.Height);
            var scaledBounds = new RectangleF(0, 0, virtualBounds.Width * _scale, virtualBounds.Height * _scale);
            _offset = dc.Bounds.Center - scaledBounds.Center;
            virtualBounds = new RectangleF(Padding.Left, Padding.Top, VirtualSize.X, VirtualSize.Y);
            foreach (var child in Children)
                Draw(child, dc.WithBounds(virtualBounds).WithAdd(_viewAdd));
        }

        void Add(MySprite sprite)
        {
            sprite.Position = sprite.Position * _scale + _offset;
            sprite.Size *= _scale;
            if (sprite.Type == SpriteType.TEXT)
                sprite.RotationOrScale *= _scale;
            _baseDc.Add(sprite);
        }
    }
}