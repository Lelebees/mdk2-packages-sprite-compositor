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
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;

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

        /// <summary>
        /// Clones a sprite a given number of times, translating each clone with the given vector compared to the previous clone.
        /// </summary>
        /// <param name="spriteToRepeat">The sprite to clone</param>
        /// <param name="repetitions">The number of instances of the sprite there need to be</param>
        /// <param name="translationStep">The vector to translate each copy by</param>
        /// <returns>The resulting array of sprites, which includes the original sprite (although no operation has been performed on that sprite)</returns>
        public static Sprite[] RepeatTranslated(Sprite spriteToRepeat, uint repetitions, Vector2 translationStep)
        {
            if (repetitions == 0) return Array.Empty<Sprite>();
            Sprite[] resultingSprites = new Sprite[(int)repetitions];
            resultingSprites[0] = spriteToRepeat;
            for (var steps = 1; steps < repetitions; steps++)
            {
                var newSprite = spriteToRepeat.Clone();
                newSprite.Translate(translationStep * steps);
                resultingSprites[steps] = newSprite;
            }

            return resultingSprites;
        }

        /// <summary>
        /// Clones a sprite a given number of times, scaling each clone with the given scalar compared to the previous clone.
        /// </summary>
        /// <param name="spriteToRepeat">The sprite to clone</param>
        /// <param name="repetitions">Number of instances of the sprite there need to be</param>
        /// <param name="scaleAnchor">The center of rotation</param>
        /// <param name="scaleStep">scalar to scale each clone by</param>
        /// <returns>The resulting list of sprites, which includes the original sprite (though no operation has been applied to that sprite)</returns>
        public static Sprite[] RepeatScaled(Sprite spriteToRepeat, uint repetitions, Vector2 scaleStep,
            Anchor scaleAnchor = null)
        {
            if (repetitions == 0) return Array.Empty<Sprite>();
            Sprite[] resultingSprites = new Sprite[(int)repetitions];
            resultingSprites[0] = spriteToRepeat;
            for (var steps = 1; steps < repetitions; steps++)
            {
                var newSprite = spriteToRepeat.Clone();
                newSprite.Scale(scaleStep * steps, scaleAnchor);
                resultingSprites[steps] = newSprite;
            }

            return resultingSprites;
        }
        
        /// <summary>
        /// Clones a sprite a given number of times, scaling each clone with the given scalar compared to the previous clone.
        /// </summary>
        /// <param name="spriteToRepeat">The sprite to clone</param>
        /// <param name="repetitions">Number of instances of the sprite there need to be</param>
        /// <param name="scaleAnchor">The center of rotation</param>
        /// <param name="scalarStep">scalar to scale each clone by</param>
        /// <returns>The resulting list of sprites, which includes the original sprite (though no operation has been applied to that sprite)</returns>
        public static Sprite[] RepeatScaled(Sprite spriteToRepeat, uint repetitions, float scalarStep, Anchor scaleAnchor = null) 
            => RepeatScaled(spriteToRepeat, repetitions, new Vector2(scalarStep, scalarStep), scaleAnchor);

        public static TextureSprite.TextureSpriteBuilder WithTexture(string texture) =>
            new TextureSprite.TextureSpriteBuilder().Texture(texture);

        public static TextSprite.TextSpriteBuilder WithText(string text) =>
            new TextSprite.TextSpriteBuilder().Text(text);

        public static ClippingSprite.ClippingSpriteBuilder ClipRectangle() =>
            new ClippingSprite.ClippingSpriteBuilder();

        public static CompositeSprite Compose(params Sprite[] sprites)
        {
            var list = new List<MySprite>(sprites.Length);
            foreach (var sprite in sprites)
            {
                list.AddRange(sprite.AsDrawableCollection());
            }

            return new CompositeSprite(list.ToArray());
        }

        /// <summary>
        /// Creates a clone of the passed sprite, which is mirrored in both dimensions.
        /// </summary>
        /// <param name="sprite">The sprite to be mirrored</param>
        /// <param name="anchor">Point in space to place the mirror. This allows you to mirror not just the sprite, but it's placement as well</param>
        /// <returns>A mirrored copy of the passed sprite</returns>
        public static Sprite Mirrored(Sprite sprite, Anchor anchor = null)
        {
            Sprite clone = sprite.Clone();
            clone.Scale(new Vector2(-1, -1), anchor);
            return clone;
        }

        /// <summary>
        /// Creates a clone of the passed sprite, which is mirrored along the Y-axis.
        /// </summary>
        /// <param name="sprite">The sprite to be mirrored</param>
        /// <param name="anchor">Point in space to place the mirror. This allows you to mirror not just the sprite, but it's placement as well</param>
        /// <returns>A mirrored copy of the passed sprite</returns>
        public static Sprite MirroredVertical(Sprite sprite, Anchor anchor = null)
        {
            Sprite clone = sprite.Clone();
            clone.Scale(new Vector2(1, -1), anchor);
            return clone;
        }

        /// <summary>
        /// Creates a clone of the passed sprite, which is mirrored along the X-axis.
        /// </summary>
        /// <param name="sprite">The sprite to be mirrored</param>
        /// <param name="anchor">Point in space to place the mirror. This allows you to mirror not just the sprite, but it's placement as well</param>
        /// <returns>A mirrored copy of the passed sprite</returns>
        public static Sprite MirroredHorizontal(Sprite sprite, Anchor anchor = null)
        {
            Sprite clone = sprite.Clone();
            clone.Scale(new Vector2(-1, 1), anchor);
            return clone;
        }
    }
}