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
    public class ClippingSprite : SpriteLeaf
    {
        private ClippingSprite(MySprite sprite) : base(sprite)
        {
        }

        public ClippingSprite() : base(new MySprite(type: SpriteType.CLIP_RECT))
        {
        }

        public override void Scale(float scalar, Anchor anchor = null) => Scale(new Vector2(scalar, scalar), anchor);

        public override void Scale(Vector2 scalar, Anchor anchor = null)
        {
            if (Sprite.Size == null) Sprite.Size = Vector2.One;

            var size = Sprite.Size.Value;
            size.X *= scalar.X;
            size.Y *= scalar.Y;
            Sprite.Size = size;
            base.Scale(scalar, anchor);
        }

        public override Sprite Clone() => new ClippingSprite(Sprite);
        
        public class ClippingSpriteBuilder
        {
            private MySprite sprite = new MySprite(type: SpriteType.CLIP_RECT);

            public ClippingSpriteBuilder Position(float x, float y) => Position(new Vector2(x, y));
            
            public ClippingSpriteBuilder Position(Vector2 position)
            {
                sprite.Position = position;
                return this;
            }

            public ClippingSpriteBuilder Rotation(Angle rotation)
            {
                sprite.RotationOrScale = (float) rotation.Radians;
                return this;
            }

            public ClippingSpriteBuilder Size(float width, float height) => Size(new Vector2(width, height));

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

            public ClippingSprite Build() => new ClippingSprite(sprite);
        }
    }
}