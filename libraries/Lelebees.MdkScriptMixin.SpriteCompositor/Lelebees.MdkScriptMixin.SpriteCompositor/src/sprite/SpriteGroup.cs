/* Copyright (c) 2026 Lelebees
   This file is part of Sprite Compositor.

   Sprite Compositor is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation,
   either version 3 of the License, or (at your option) any later version.

   Sprite Compositor is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
   See the GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License along with Sprite Compositor.
   If not, see <https://www.gnu.org/licenses/>. */

using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public abstract class SpriteGroup : Sprite
    {
        public Vector2 GetPosition()
        {
            var children = GetChildren();
            Vector2 result = Vector2.Zero;
            foreach (var child in children) result += child.GetPosition();
            return result / children.Count;
        }

        public void Translate(Vector2 vector)
        {
            foreach (var sprite in GetChildren()) sprite.Translate(vector);
        }

        public void Translate(float x, float y) => Translate(new Vector2(x, y));

        public void SetColor(Color color)
        {
            foreach (var sprite in GetChildren()) sprite.SetColor(color);
        }

        public void SetAlignment(TextAlignment alignment)
        {
            foreach (var sprite in GetChildren()) sprite.SetAlignment(alignment);
        }

        public void Scale(float scalar, Anchor anchor = null) => Scale(new Vector2(scalar, scalar), anchor);

        public void Scale(Vector2 scalar, Anchor anchor = null)
        {
            // See the rotate function for why we're doing this.
            if (anchor == null)
            {
                anchor = new PointAnchor(GetPosition());
            }
            else if (anchor is SpriteGroup)
            {
                anchor = new PointAnchor(anchor.GetPosition());
            }

            foreach (var sprite in GetChildren()) sprite.Scale(scalar, anchor);
        }

        public void Mirror(Anchor anchor = null) => Scale(new Vector2(-1, -1), anchor);

        public void MirrorVertical(Anchor anchor = null) => Scale(new Vector2(1, -1), anchor);

        public void MirrorHorizontal(Anchor anchor = null) => Scale(new Vector2(-1, 1), anchor);

        public void Rotate(Angle angle, Anchor positionAnchor = null)
        {
            // it is VITALLY important to create a PointAnchor here. If we don't,
            // the process of rotating sprites could adjust this (or another) groups center, and therefore its position
            // this causes sprites to spiral away from the center of rotation.
            if (positionAnchor == null)
            {
                positionAnchor = new PointAnchor(GetPosition());
            }
            else if (positionAnchor is SpriteGroup)
            {
                positionAnchor = new PointAnchor(positionAnchor.GetPosition());
            }

            foreach (var sprite in GetChildren())
            {
                sprite.Rotate(angle, positionAnchor);
            }
        }

        public abstract Sprite Clone();

        public MySprite[] AsDrawableCollection(RectangleF viewport)
        {
            List<MySprite> list = new List<MySprite>();
            foreach (Sprite child in GetChildren())
            foreach (MySprite sprite in child.AsDrawableCollection(viewport))
                list.Add(sprite);
            return list.ToArray();
        }

        public MySprite[] AsDrawableCollection()
        {
            List<MySprite> list = new List<MySprite>();
            foreach (Sprite child in GetChildren())
            foreach (MySprite sprite in child.AsDrawableCollection())
                list.Add(sprite);
            return list.ToArray();
        }

        protected abstract List<Sprite> GetChildren();
    }
}