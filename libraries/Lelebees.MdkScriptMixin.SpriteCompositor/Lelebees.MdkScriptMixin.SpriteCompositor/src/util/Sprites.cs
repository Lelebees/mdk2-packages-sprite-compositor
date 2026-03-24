using System;
using System.Collections.Generic;

namespace IngameScript
{
    public abstract class Sprites
    {
        /// <summary>
        /// Clones a sprite timesToRepeat times, places them in a circle and divides the space between sprites evenly.  
        /// </summary>
        /// <param name="spriteToRepeat">The sprite to clone</param>
        /// <param name="timesToRepeat">Number of instances of the sprite there need to be</param>
        /// <param name="rotationAnchor">The center of rotation</param>
        /// <returns>The resulting list of sprites, which includes the original sprite (though no operation has been applied to that sprite)</returns>
        public static List<Sprite> RepeatRotated(Sprite spriteToRepeat, int timesToRepeat, Anchor rotationAnchor)
        {
            double anglePerItem = 2 * Math.PI / timesToRepeat;
            Angle stepSize = new Angle(anglePerItem, AngleUnit.Radians);
            return RepeatRotated(spriteToRepeat, timesToRepeat, rotationAnchor, stepSize);
        }

        /// <summary>
        /// Clones a sprite timesToRepeat times, rotating each clone at an angle of stepSize compared to the previous clone.
        /// </summary>
        /// <param name="spriteToRepeat">The sprite to clone</param>
        /// <param name="timesToRepeat">Number of instances of the sprite there need to be</param>
        /// <param name="rotationAnchor">The center of rotation</param>
        /// <param name="stepSize">The angle to rotate each clone compared to the previous one</param>
        /// <returns>The resulting list of sprites, which includes the original sprite (though no operation has been applied to that sprite)</returns>
        public static List<Sprite> RepeatRotated(Sprite spriteToRepeat, int timesToRepeat, Anchor rotationAnchor,
            Angle stepSize)
        {
            List<Sprite> resultingSprites = new List<Sprite> { spriteToRepeat };
            for (var i = 1; i < timesToRepeat; i++)
            {
                var newAngle = new Angle(stepSize.AsRadians() * i, AngleUnit.Radians);
                var newSprite = spriteToRepeat.Clone();
                newSprite.Rotate(newAngle, rotationAnchor);
                resultingSprites.Add(newSprite);
            }

            return resultingSprites;
        }
    }
}