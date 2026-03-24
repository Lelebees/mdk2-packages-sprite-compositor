using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class TextureSprite : SpriteLeaf
    {
        public TextureSprite(string texturePath = null) : base(new MySprite(type: SpriteType.TEXTURE,
            position: Vector2.Zero, rotation: 0, data: texturePath))
        {
        }

        private TextureSprite(MySprite sprite) : base(sprite)
        {
        }

        public void SetTexture(string texturePath)
        {
            Sprite.Data = texturePath;
        }

        public override void Scale(float amount)
        {
            if (Sprite.Size == null)
            {
                Sprite.Size = Vector2.One;
            }

            var size = Sprite.Size.Value;
            size.X *= amount;
            size.Y *= amount;
            Sprite.Size = size;
        }

        public override void Rotate(Angle angle, Anchor anchor = null)
        {
            if (anchor == null)
            {
                anchor = this;
            }

            var cos = Math.Cos(-1 * angle.AsRadians());
            var sin = Math.Sin(-1 * angle.AsRadians());

            var anchorX = anchor.GetPosition().X;
            var anchorY = anchor.GetPosition().Y;

            var position = Sprite.Position ?? Vector2.Zero;
            var posX = (float)(cos * (position.X - anchorX) + (position.Y - anchorY) * sin + anchorX);
            var posY = (float)(-1 * sin * (position.X - anchorX) + cos * (position.Y - anchorY) + anchorY);
            Sprite.Position = new Vector2(posX, posY);
            Sprite.RotationOrScale += (float)angle.AsRadians();
        }

        public override Sprite Clone()
        {
            return new TextureSprite(Sprite);
        }

        public void SetSize(Vector2 size)
        {
            Sprite.Size = size;
        }
    }
}