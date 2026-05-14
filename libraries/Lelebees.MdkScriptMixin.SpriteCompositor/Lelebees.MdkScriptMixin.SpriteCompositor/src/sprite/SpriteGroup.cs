using System.Collections.Generic;
using System.Linq;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public abstract class SpriteGroup : Sprite
    {
        public Vector2 GetPosition()
        {
            var children = GetChildren();
            return children.Aggregate(Vector2.Zero, (current, child) => current + child.GetPosition()) / children.Count;
        }

        public void Translate(Vector2 vector) => GetChildren().ForEach(sprite => sprite.Translate(vector));

        public void Translate(float x, float y) => Translate(new Vector2(x, y));

        public void SetColor(Color color) => GetChildren().ForEach(sprite => sprite.SetColor(color));

        public void SetAlignment(TextAlignment alignment) =>
            GetChildren().ForEach(sprite => sprite.SetAlignment(alignment));

        public void Scale(float scalar, Anchor anchor = null) => Scale(new Vector2(scalar, scalar), anchor);

        public void Scale(Vector2 scalar, Anchor anchor = null)
        {
            // See the rotate function for why we're doing this.
            if (anchor == null)
            {
                anchor = new PointAnchor(GetPosition());
            }
            else if (anchor is SpriteGroup)
            {
                anchor = new PointAnchor(anchor.GetPosition());
            }

            GetChildren().ForEach(sprite => sprite.Scale(scalar, anchor));
        }

        public void Mirror(Anchor anchor = null) => Scale(new Vector2(-1, -1), anchor);

        public void MirrorVertical(Anchor anchor = null) => Scale(new Vector2(1, -1), anchor);

        public void MirrorHorizontal(Anchor anchor = null) => Scale(new Vector2(-1, 1), anchor);

        public void Rotate(Angle angle, Anchor positionAnchor = null)
        {
            // it is VITALLY important to create a PointAnchor here. If we don't,
            // the process of rotating sprites could adjust this (or another) groups center, and therefore its position
            // this causes sprites to spiral away from the center of rotation.
            if (positionAnchor is SpriteGroup)
            {
                positionAnchor = new PointAnchor(positionAnchor.GetPosition());
            }
            GetChildren().ForEach(sprite => sprite.Rotate(angle, positionAnchor ?? new PointAnchor(GetPosition())));
        }

        public abstract Sprite Clone();

        public MySprite[] AsDrawableCollection(RectangleF viewport) =>
            GetChildren().SelectMany(child => child.AsDrawableCollection(viewport)).ToArray();

        public MySprite[] AsDrawableCollection() =>
            GetChildren().SelectMany(child => child.AsDrawableCollection()).ToArray();

        protected abstract List<Sprite> GetChildren();
    }
}