/* Copyright (c) 2026 Lelebees
   This file is part of Sprite Composer.

   Sprite Composer is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation,
   either version 3 of the License, or (at your option) any later version.

   Sprite Composer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
   See the GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License along with Sprite Composer.
   If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.Linq;

namespace IngameScript
{
    public abstract class Sprites
    {
        /// <summary>
        /// Clones a sprite a given number of times, places them in a circle and divides the space between sprites evenly.  
        /// </summary>
        /// <param name="spriteToRepeat">The sprite to clone</param>
        /// <param name="repetitions">Number of instances of the sprite there need to be</param>
        /// <param name="rotationAnchor">The center of rotation</param>
        /// <returns>The resulting list of sprites, which includes the original sprite (though no operation has been applied to that sprite)</returns>
        public static Sprite[]
            RepeatRotated(Sprite spriteToRepeat, uint repetitions, Anchor rotationAnchor = null) =>
            RepeatRotated(spriteToRepeat, repetitions, Angle.Full / repetitions, rotationAnchor);

        /// <summary>
        /// Clones a sprite a given number of times, rotating each clone with the given angle compared to the previous clone.
        /// </summary>
        /// <param name="spriteToRepeat">The sprite to clone</param>
        /// <param name="repetitions">Number of instances of the sprite there need to be</param>
        /// <param name="rotationAnchor">The center of rotation</param>
        /// <param name="stepSize">The angle to rotate each clone compared to the previous one</param>
        /// <returns>The resulting list of sprites, which includes the original sprite (though no operation has been applied to that sprite)</returns>
        public static Sprite[] RepeatRotated(Sprite spriteToRepeat, uint repetitions, Angle stepSize,
            Anchor rotationAnchor = null)
        {
            if (repetitions == 0) return Array.Empty<Sprite>();
            Sprite[] resultingSprites = new Sprite[(int)repetitions];
            resultingSprites[0] = spriteToRepeat;
            for (var steps = 1; steps < repetitions; steps++)
            {
                var newSprite = spriteToRepeat.Clone();
                newSprite.Rotate(stepSize * steps, rotationAnchor);
                resultingSprites[steps] = newSprite;
            }

            return resultingSprites;
        }

        public static TextureSprite.TextureSpriteBuilder WithTexture(string texture) => new TextureSprite.TextureSpriteBuilder().Texture(texture);

        public static TextSprite.TextSpriteBuilder WithText(string text) => new TextSprite.TextSpriteBuilder().Text(text);

        public static ClippingSprite.ClippingSpriteBuilder ClipRectangle() => new ClippingSprite.ClippingSpriteBuilder();

        public static SpriteGroup Group(params Sprite[] sprites) => new SimpleSpriteGroup(sprites.ToList());
    }
}