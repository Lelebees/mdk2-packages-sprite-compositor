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

        public void SetScale(float scale)
        {
            Sprite.RotationOrScale = scale;
        }

        public override void Scale(float scalar, Anchor anchor = null)
        {
            Scale(new Vector2(scalar, scalar), anchor);
        }

        public override void Scale(Vector2 scalar, Anchor anchor = null)
        {
            // Text sprites don't really scale in 2 dimensions, so instead we'll take the average scale
            Sprite.RotationOrScale = (scalar.X + scalar.Y) / 2;
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
            if (anchor == null || anchor == this)
            {
                return;
            }

            var cos = Math.Cos(-angle.AsRadians());
            var sin = Math.Sin(-angle.AsRadians());

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
        
        public class TextSpriteBuilder
        {
            private MySprite sprite = new MySprite(type: SpriteType.TEXT);

            public TextSpriteBuilder Text(string text)
            {
                sprite.Data = text;
                return this;
            }

            public TextSpriteBuilder FontId(string fontId)
            {
                sprite.FontId = fontId;
                return this;
            }

            public TextSpriteBuilder Position(float x, float y)
            {
                return Position(new Vector2(x, y));
            }
            
            public TextSpriteBuilder Position(Vector2 position)
            {
                sprite.Position = position;
                return this;
            }

            public TextSpriteBuilder Scale(float scale)
            {
                sprite.RotationOrScale = scale;
                return this;
            }

            public TextSpriteBuilder Color(Color color)
            {
                sprite.Color = color;
                return this;
            }

            public TextSpriteBuilder Alignment(TextAlignment alignment)
            {
                sprite.Alignment = alignment;
                return this;
            }

            public TextSprite Build()
            {
                return new TextSprite(sprite);
            }
        }

        public static TextSpriteBuilder Builder()
        {
            return new TextSpriteBuilder();
        }
    }
}