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
            Sprite.RotationOrScale *= (scalar.X + scalar.Y) / 2;
            base.Scale(scalar, anchor);
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
    }
}