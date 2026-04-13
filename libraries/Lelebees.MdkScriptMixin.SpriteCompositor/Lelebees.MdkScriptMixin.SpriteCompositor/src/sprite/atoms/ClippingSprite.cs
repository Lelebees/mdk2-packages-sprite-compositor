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

        public override void Scale(float scalar, Anchor anchor = null)
        {
            Scale(new Vector2(scalar, scalar), anchor);
        }

        public override void Scale(Vector2 scalar, Anchor anchor = null)
        {
            if (Sprite.Size == null) Sprite.Size = Vector2.One;

            var size = Sprite.Size.Value;
            size.X *= scalar.X;
            size.Y *= scalar.Y;
            Sprite.Size = size;
            base.Scale(scalar, anchor);
        }

        public override Sprite Clone()
        {
            return new ClippingSprite(Sprite);
        }
        
        public class ClippingSpriteBuilder
        {
            private MySprite sprite = new MySprite(type: SpriteType.CLIP_RECT);

            public ClippingSpriteBuilder Position(float x, float y)
            {
                return Position(new Vector2(x, y));
            }
            
            public ClippingSpriteBuilder Position(Vector2 position)
            {
                sprite.Position = position;
                return this;
            }

            public ClippingSpriteBuilder Rotation(Angle rotation)
            {
                sprite.RotationOrScale = (float) rotation.AsRadians();
                return this;
            }

            public ClippingSpriteBuilder Size(float width, float height)
            {
                return Size(new Vector2(width, height));
            }

            public ClippingSpriteBuilder Size(Vector2 size)
            {
                sprite.Size = size;
                return this;
            }

            public ClippingSpriteBuilder Alignment(TextAlignment alignment)
            {
                sprite.Alignment = alignment;
                return this;
            }

            public ClippingSprite Build()
            {
                return new ClippingSprite(sprite);
            }
        }
    }
}