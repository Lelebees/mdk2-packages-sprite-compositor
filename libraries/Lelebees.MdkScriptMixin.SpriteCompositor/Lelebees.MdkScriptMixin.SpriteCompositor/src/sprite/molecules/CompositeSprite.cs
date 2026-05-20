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
    public class CompositeSprite : Sprite
    {
        protected readonly MySprite[] Children;
        private Vector2? position;
        private readonly MySprite[] drawable;

        public CompositeSprite(MySprite[] children)
        {
            Children = children;
            drawable = new MySprite[Children.Length];
        }

        public Vector2 GetPosition()
        {
            if (!position.HasValue)
            {
                Vector2 result = Vector2.Zero;
                foreach (var child in Children) result += child.Position ?? Vector2.Zero;
                return result / Children.Length;
            }
            return position.Value;
        }

        public virtual void Translate(Vector2 vector)
        {
            for (var index = 0; index < Children.Length; index++)
            {
                var childPosition = Children[index].Position ?? Vector2.Zero;
                childPosition += vector;
                Children[index].Position = childPosition;
            }

            position = null;
        }

        public void Translate(float x, float y) => Translate(new Vector2(x, y));

        public virtual void SetColor(Color color)
        {
            for (var index = 0; index < Children.Length; index++) Children[index].Color = color;
        }

        public virtual void SetAlignment(TextAlignment alignment)
        {
            for (var index = 0; index < Children.Length; index++) Children[index].Alignment = alignment;
        }

        public void Scale(float scalar, Anchor anchor = null) => Scale(new Vector2(scalar, scalar), anchor);

        public virtual void Scale(Vector2 scalar, Anchor anchor = null)
        {
            var anchorPos = GetPosition();
            if (anchor != null)
            {
                anchorPos = anchor.GetPosition();
            }

            for (var index = 0; index < Children.Length; index++)
            {
                switch (Children[index].Type)
                {
                    case SpriteType.TEXT:
                        Children[index].RotationOrScale *= (scalar.X + scalar.Y) / 2;
                        break;
                    case SpriteType.TEXTURE:
                    case SpriteType.CLIP_RECT:
                        var size = Children[index].Size ?? Vector2.One;
                        size.X *= scalar.X;
                        size.Y *= scalar.Y;
                        Children[index].Size = size;
                        break;
                }

                if (anchorPos == Children[index].Position) continue;

                var distanceFromAnchor = Children[index].Position ?? Vector2.Zero - anchorPos;
                distanceFromAnchor *= scalar;
                Children[index].Position = anchorPos + distanceFromAnchor;
            }
            position = null;
        }

        public virtual void Rotate(Angle angle, Anchor positionAnchor = null)
        {
            var anchor = GetPosition();
            if (positionAnchor != null) anchor = positionAnchor.GetPosition();
            for (var index = 0; index < Children.Length; index++)
            {
                if (Children[index].Type != SpriteType.TEXT)
                {
                    Children[index].RotationOrScale += (float)angle.Radians;
                }

                if (anchor == Children[index].Position) continue;

                var cos = Math.Cos(-angle.Radians);
                var sin = Math.Sin(-angle.Radians);

                var childPosition = Children[index].Position ?? Vector2.Zero;
                var x = (float)(cos * (childPosition.X - anchor.X) + sin * (childPosition.Y - anchor.Y) + anchor.X);
                var y = (float)(-sin * (childPosition.X - anchor.X) + cos * (childPosition.Y - anchor.Y) + anchor.Y);
                Children[index].Position = new Vector2(x, y);
            }
            position = null;
        }

        public virtual MySprite[] AsDrawableCollection(RectangleF viewport)
        {
            var drawables = AsDrawableCollection();
            for (var index = 0; index < drawables.Length; index++)
            {
                drawables[index].Position += viewport.Center;
            }

            return drawables;
        }
        
        public virtual MySprite[] AsDrawableCollection()
        {
            Children.CopyTo(drawable, 0);
            return drawable;
        }


        public virtual Sprite Clone()
        {
            var newArray = new MySprite[Children.Length];
            Children.CopyTo(newArray, 0);
            return new CompositeSprite(newArray);
        }
    }
}