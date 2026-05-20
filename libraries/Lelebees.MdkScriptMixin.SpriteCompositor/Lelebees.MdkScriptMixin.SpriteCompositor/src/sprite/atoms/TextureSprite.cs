/* Copyright (c) 2026 Lelebees
   This file is part of Sprite Compositor.

   Sprite Compositor is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation,
   either version 3 of the License, or (at your option) any later version.

   Sprite Compositor is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
   See the GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License along with Sprite Compositor.
   If not, see <https://www.gnu.org/licenses/>. */

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

        private TextureSprite(MySprite sprite) : base(sprite)
        {
        }

        public override void Scale(Vector2 scalar, Anchor anchor = null)
        {
            var size = Sprite.Size ?? Vector2.One;
            size.X *= scalar.X;
            size.Y *= scalar.Y;
            Sprite.Size = size;
            base.Scale(scalar, anchor);
        }

        public override void Rotate(Angle angle, Anchor positionAnchor = null)
        {
            Sprite.RotationOrScale += (float)angle.Radians;
            base.Rotate(angle, positionAnchor);
        }

        public override Sprite Clone() => new TextureSprite(Sprite);
        

        public class TextureSpriteBuilder
        {
            private MySprite sprite = new MySprite(type: SpriteType.TEXTURE);

            public TextureSpriteBuilder Texture(string path)
            {
                sprite.Data = path;
                return this;
            }

            public TextureSpriteBuilder Position(float x, float y) => Position(new Vector2(x, y));

            public TextureSpriteBuilder Position(Vector2 position)
            {
                sprite.Position = position;
                return this;
            }

            public TextureSpriteBuilder Rotation(Angle rotation)
            {
                sprite.RotationOrScale = (float)rotation.Radians;
                return this;
            }

            public TextureSpriteBuilder Size(float width, float height) => Size(new Vector2(width, height));

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

            public TextureSprite Build() => new TextureSprite(sprite);
        }
    }
}