using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class TextSprite : SpriteLeaf
    {
        public TextSprite(string text) : base(new MySprite(type: SpriteType.TEXT, data: text))
        {
        }

        private TextSprite(MySprite sprite) : base(sprite)
        {
            
        }


        public void SetText(string text)
        {
            Sprite.Data = text;
        }

        public void SetFontId(string fontId)
        {
            Sprite.FontId = fontId;
        }

        public override void Scale(float amount, Anchor anchor = null)
        {
            Sprite.RotationOrScale = amount;
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
            if (anchor == null || anchor == this)
            {
                return;
            }

            var cos = Math.Cos(angle.AsRadians());
            var sin = Math.Sin(angle.AsRadians());

            var anchorX = anchor.GetPosition().X;
            var anchorY = anchor.GetPosition().Y;

            var position = Sprite.Position ?? Vector2.Zero;
            position.X = (float)(cos * (position.X - anchorX) + (position.Y + anchorY) * sin + anchorX);
            position.Y = (float)(-1 * sin * (position.X - anchorX) + cos * (position.Y - anchorY) + anchorY);
            Sprite.Position = position;
        }

        public override Sprite Clone()
        {
            return new TextSprite(Sprite);
        }
    }
}