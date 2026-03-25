using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class ClippingSprite : SpriteLeaf
    {
        private ClippingSprite(MySprite sprite) : base(sprite)
        {
        }

        public ClippingSprite() : base(new MySprite(type: SpriteType.CLIP_RECT))
        {
            
        }

        public override void Scale(float amount, Anchor anchor = null)
        {
            if (Sprite.Size == null)
            {
                Sprite.Size = Vector2.One;
            }

            var size = Sprite.Size.Value;
            size.X *= amount;
            size.Y *= amount;
            Sprite.Size = size;
            if (anchor == null || anchor == this || anchor.GetPosition() == GetPosition())
            {
                return;
            }

            var anchorPos = anchor.GetPosition();
            var distanceFromAnchor = this.GetPosition() - anchorPos;
            distanceFromAnchor *= amount;
            Sprite.Position =  anchorPos + distanceFromAnchor;
        }

        public override void Rotate(Angle angle, Anchor anchor = null)
        {
            if (anchor == null)
            {
                anchor = this;
            }

            var cos = Math.Cos(angle.AsRadians());
            var sin = Math.Sin(angle.AsRadians());

            var anchorX = anchor.GetPosition().X;
            var anchorY = anchor.GetPosition().Y;

            var position = Sprite.Position ?? Vector2.Zero;
            position.X = (float)(cos * (position.X - anchorX) + (position.Y + anchorY) * sin + anchorX);
            position.Y = (float)(-1 * sin * (position.X - anchorX) + cos * (position.Y - anchorY) + anchorY);
            Sprite.Position = position;
            Sprite.RotationOrScale = (float)angle.AsRadians();
        }

        public override Sprite Clone()
        {
            return new ClippingSprite(Sprite);
        }
    }
}