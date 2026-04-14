using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class TextureSprite : SpriteLeaf
    {
        public float Rotation
        {
            get { return Sprite.RotationOrScale; }
            set { Sprite.RotationOrScale = value; }
        }

        public Vector2? Size
        {
            get { return Sprite.Size; }
            set { Sprite.Size = value; }
        }

        public string Texture
        {
            get { return Sprite.Data; }
            set { Sprite.Data = value; }
        }


        // public TextureSprite(string texturePath = null) : base(new MySprite(type: SpriteType.TEXTURE,
        //     position: Vector2.Zero, rotation: 0, data: texturePath))
        // {
        // }

        private TextureSprite(MySprite sprite) : base(sprite)
        {
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
            base.Scale(scalar, anchor);
        }

        public override void Rotate(Angle angle, Anchor positionAnchor = null)
        {
            Sprite.RotationOrScale += (float)angle.AsRadians();
            base.Rotate(angle, positionAnchor);
        }

        public override Sprite Clone()
        {
            return new TextureSprite(Sprite);
        }

        public class TextureSpriteBuilder
        {
            private MySprite sprite = new MySprite(type: SpriteType.TEXTURE);

            public TextureSpriteBuilder Texture(string path)
            {
                sprite.Data = path;
                return this;
            }

            public TextureSpriteBuilder Position(float x, float y)
            {
                return Position(new Vector2(x, y));
            }

            public TextureSpriteBuilder Position(Vector2 position)
            {
                sprite.Position = position;
                return this;
            }

            public TextureSpriteBuilder Rotation(Angle rotation)
            {
                sprite.RotationOrScale = (float)rotation.AsRadians();
                return this;
            }

            public TextureSpriteBuilder Size(float width, float height)
            {
                return Size(new Vector2(width, height));
            }

            public TextureSpriteBuilder Size(Vector2 size)
            {
                sprite.Size = size;
                return this;
            }

            public TextureSpriteBuilder Color(Color color)
            {
                sprite.Color = color;
                return this;
            }

            public TextureSpriteBuilder Alignment(TextAlignment alignment)
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