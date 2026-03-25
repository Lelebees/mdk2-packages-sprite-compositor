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

        public override void Scale(float scalar, Anchor anchor = null)
        {
            Scale(new Vector2(scalar, scalar), anchor);
        }

        public override void Scale(Vector2 scalar, Anchor anchor = null)
        {
            if (Sprite.Size == null)
            {
                Sprite.Size = Vector2.One;
            }

            var size = Sprite.Size.Value;
            size.X *= scalar.X;
            size.Y *= scalar.Y;
            Sprite.Size = size;
            if (anchor == null || anchor == this || anchor.GetPosition() == GetPosition())
            {
                return;
            }

            var anchorPos = anchor.GetPosition();
            var distanceFromAnchor = this.GetPosition() - anchorPos;
            distanceFromAnchor *= scalar;
            Sprite.Position = anchorPos + distanceFromAnchor;
        }

        public override void Rotate(Angle angle, Anchor anchor = null)
        {
            Sprite.RotationOrScale += (float)angle.AsRadians();
            
            if (anchor == null || anchor == this || anchor.GetPosition() == GetPosition()) return;

            var cos = Math.Cos(-angle.AsRadians());
            var sin = Math.Sin(-angle.AsRadians());

            var anchorPos = anchor.GetPosition();

            var position = Sprite.Position ?? Vector2.Zero;
            var posX = (float)(cos * (position.X - anchorPos.X) + (position.Y - anchorPos.Y) * sin + anchorPos.X);
            var posY = (float)(-1 * sin * (position.X - anchorPos.X) + cos * (position.Y - anchorPos.Y) + anchorPos.Y);
            Sprite.Position = new Vector2(posX, posY);
        }

        public override Sprite Clone()
        {
            return new TextureSprite(Sprite);
        }

        public void SetSize(Vector2 size)
        {
            Sprite.Size = size;
        }

        public static Builder GetBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            private MySprite sprite = new MySprite(type: SpriteType.TEXTURE);

            public Builder Texture(string path)
            {
                sprite.Data = path;
                return this;
            }

            public Builder Position(float x, float y)
            {
                return Position(new Vector2(x, y));
            }
            
            public Builder Position(Vector2 position)
            {
                sprite.Position = position;
                return this;
            }

            public Builder Rotation(Angle rotation)
            {
                sprite.RotationOrScale = (float) rotation.AsRadians();
                return this;
            }

            public Builder Size(float width, float height)
            {
                return Size(new Vector2(width, height));
            }

            public Builder Size(Vector2 size)
            {
                sprite.Size = size;
                return this;
            }

            public Builder Color(Color color)
            {
                sprite.Color = color;
                return this;
            }

            public Builder Alignment(TextAlignment alignment)
            {
                sprite.Alignment = alignment;
                return this;
            }

            public TextureSprite Build()
            {
                return new TextureSprite(sprite);
            }
        }
    }
}