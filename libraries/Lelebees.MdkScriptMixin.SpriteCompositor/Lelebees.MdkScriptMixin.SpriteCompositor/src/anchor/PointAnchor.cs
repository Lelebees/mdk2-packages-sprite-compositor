/* Copyright (c) 2026 Lelebees
   This file is part of Sprite Composer.

   Sprite Composer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation,
   either version 3 of the License, or (at your option) any later version.

   Sprite Composer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
   See the GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License along with Sprite Composer.
   If not, see <https://www.gnu.org/licenses/>. */

using VRageMath;

namespace IngameScript
{
    public struct PointAnchor : Anchor
    {
        private readonly Vector2 position;

        public PointAnchor(Vector2 position)
        {
            this.position = position;
        }

        public PointAnchor(float x, float y) : this(new Vector2(x, y)) {}

        public Vector2 GetPosition()
        {
            return position;
        }
    }
}