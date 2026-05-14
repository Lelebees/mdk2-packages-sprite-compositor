using System;
using System.Collections.Generic;
using System.Linq;
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

        public static TextureSprite.TextureSpriteBuilder WithTexture(string texture) => new TextureSprite.TextureSpriteBuilder().Texture(texture);

        public static TextSprite.TextSpriteBuilder WithText(string text) => new TextSprite.TextSpriteBuilder().Text(text);

        public static ClippingSprite.ClippingSpriteBuilder ClipRectangle() => new ClippingSprite.ClippingSpriteBuilder();

        public static SpriteGroup Group(params Sprite[] sprites) => new SimpleSpriteGroup(sprites.ToList());
    }
}