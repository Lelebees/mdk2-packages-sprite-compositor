/* Copyright (c) 2026 Lelebees
   This file is part of Sprite Compositor.

   Sprite Compositor is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation,
   either version 3 of the License, or (at your option) any later version.

   Sprite Compositor is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
   See the GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License along with Sprite Compositor.
   If not, see <https://www.gnu.org/licenses/>. */

using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public abstract class SpriteLeaf : Sprite
    {
        protected MySprite Sprite;

        public Vector2? Position
        {
            get { return Sprite.Position; }
            set { Sprite.Position = value; }
        }

        protected SpriteLeaf(MySprite sprite)
        {
            Sprite = sprite;
        }

        public Vector2 GetPosition()
        {
            return Sprite.Position ?? Vector2.Zero;
        }

        public void Translate(Vector2 vector)
        {
            var position = GetPosition();
            position.X += vector.X;
            position.Y += vector.Y;
            Sprite.Position = position;
        }

        public void Translate(float x, float y)
        {
            Translate(new Vector2(x, y));
        }

        public void SetColor(Color color)
        {
            Sprite.Color = color;
        }

        public void SetAlignment(TextAlignment alignment)
        {
            Sprite.Alignment = alignment;
        }

        public virtual void Scale(float scalar, Anchor anchor = null) => Scale(new Vector2(scalar, scalar), anchor);

        public virtual void Scale(Vector2 scalar, Anchor anchor = null)
        {
            if (anchor == null || anchor == this || anchor.GetPosition() == GetPosition()) return;

            var anchorPos = anchor.GetPosition();
            var distanceFromAnchor = GetPosition() - anchorPos;
            distanceFromAnchor *= scalar;
            Sprite.Position = anchorPos + distanceFromAnchor;
        }

        public virtual void Rotate(Angle angle, Anchor positionAnchor = null)
        {
            if (positionAnchor == null || positionAnchor == this ||
                positionAnchor.GetPosition() == GetPosition()) return;
            
            var cos = Math.Cos(-angle.Radians);
            var sin = Math.Sin(-angle.Radians);

            var anchor = positionAnchor.GetPosition();

            var position = Sprite.Position ?? Vector2.Zero;
            var posX = (float)(cos * (position.X - anchor.X) + sin * (position.Y - anchor.Y) + anchor.X);
            var posY = (float)(-sin * (position.X - anchor.X) + cos * (position.Y - anchor.Y) + anchor.Y);
            Sprite.Position = new Vector2(posX, posY);
        }

        public abstract Sprite Clone();

        public MySprite[] AsDrawableCollection(RectangleF viewport)
        {
            return new[]
            {
                new MySprite(
                    Sprite.Type,
                    Sprite.Data,
                    Sprite.Position + viewport.Center,
                    Sprite.Size,
                    Sprite.Color,
                    Sprite.FontId,
                    Sprite.Alignment,
                    Sprite.RotationOrScale)
            };
        }

        public MySprite[] AsDrawableCollection()
        {
            return new[] { Sprite };
        }
    }
}