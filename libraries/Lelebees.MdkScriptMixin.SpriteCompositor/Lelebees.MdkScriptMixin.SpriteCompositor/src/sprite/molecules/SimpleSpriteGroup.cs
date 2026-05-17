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

namespace IngameScript
{
    public class SimpleSpriteGroup : SpriteGroup
    {
        private readonly List<Sprite> children;
        
        public SimpleSpriteGroup(List<Sprite> children)
        {
            this.children = children;
        }
        
        public override Sprite Clone()
        {
            List<Sprite> list = new List<Sprite>();
            foreach (var sprite in children) list.Add(sprite.Clone());

            return new SimpleSpriteGroup(list);
        }

        protected override List<Sprite> GetChildren() => children;
    }
}