/* Copyright (c) 2026 Lelebees
   This file is part of Sprite Composer.

   Sprite Composer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation,
   either version 3 of the License, or (at your option) any later version.

   Sprite Composer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
   See the GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License along with Sprite Composer.
   If not, see <https://www.gnu.org/licenses/>. */

using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class TextSprite : SpriteLeaf
    {
        public string Text
        {
            get { return Sprite.Data;}
            set { Sprite.Data = value; }
        }
        public string FontId
        {
            get { return Sprite.FontId; }
            set { Sprite.FontId = value; }
        }

        public float TextScale
        {
            get { return Sprite.RotationOrScale; }
            set { Sprite.RotationOrScale = value; }
        }

        public TextAlignment Alignment
        {
            get { return Sprite.Alignment; }
            set { Sprite.Alignment = value; }
        }

        private TextSprite(MySprite sprite) : base(sprite)
        {
        }

        public override void Scale(float scalar, Anchor anchor = null)
        {
            Sprite.RotationOrScale *= scalar;
            base.Scale(new Vector2(scalar, scalar), anchor);
        }

        public override void Scale(Vector2 scalar, Anchor anchor = null)
        {
            // Text sprites don't really scale in 2 dimensions, so instead we'll take the average scale
            Sprite.RotationOrScale *= (scalar.X + scalar.Y) / 2;
            base.Scale(scalar, anchor);
        }

        public override Sprite Clone() => new TextSprite(Sprite);
        
        public class TextSpriteBuilder
        {
            private MySprite sprite = new MySprite(type: SpriteType.TEXT, rotation: 1f);

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

            public TextSpriteBuilder Position(float x, float y) => Position(new Vector2(x, y));
            
            
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

            public TextSprite Build() => new TextSprite(sprite);
        }
    }
}