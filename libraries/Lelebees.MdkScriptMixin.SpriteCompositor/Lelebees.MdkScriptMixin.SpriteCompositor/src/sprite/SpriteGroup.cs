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
            return GetChildren().Aggregate(Vector2.Zero, (current, child) => current + child.GetPosition()) / GetChildren().Count;
        }

        public void Translate(Vector2 vector)
        {
            GetChildren().ForEach(sprite => sprite.Translate(vector));
        }

        public void Translate(float x, float y)
        {
            Translate(new Vector2(x, y));
        }

        public void SetColor(Color color)
        {
            GetChildren().ForEach(sprite => sprite.SetColor(color));
        }

        public void SetAlignment(TextAlignment alignment)
        {
            GetChildren().ForEach(sprite => sprite.SetAlignment(alignment));
        }

        public void Scale(float scalar, Anchor anchor = null)
        {
            Scale(new Vector2(scalar, scalar), anchor);
        }

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

        public void Rotate(Angle angle, Anchor positionAnchor = null)
        {
            // it is VITALLY important to create a PointAnchor here. If we don't,
            // the process of rotating sprites will adjust this groups center, and therefore it's position
            // this causes sprites to spiral away from the center of rotation.
            if (positionAnchor == null)
            {
                positionAnchor = new PointAnchor(GetPosition());
            }
            // Safeguard if someone passes this group or a subgroup of this group as an anchor.
            else if (positionAnchor is SpriteGroup)
            {
                positionAnchor = new PointAnchor(positionAnchor.GetPosition());
            }
            GetChildren().ForEach(sprite => sprite.Rotate(angle, positionAnchor));
        }

        public abstract Sprite Clone();

        public MySprite[] AsDrawableCollection(RectangleF viewport)
        {
            return GetChildren().SelectMany(child => child.AsDrawableCollection(viewport)).ToArray();
        }

        public MySprite[] AsDrawableCollection()
        {
            return GetChildren().SelectMany(child => child.AsDrawableCollection()).ToArray();
        }

        protected abstract List<Sprite> GetChildren();
    }
}