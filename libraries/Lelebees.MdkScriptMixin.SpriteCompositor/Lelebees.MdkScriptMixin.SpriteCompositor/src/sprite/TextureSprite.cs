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
    }
}