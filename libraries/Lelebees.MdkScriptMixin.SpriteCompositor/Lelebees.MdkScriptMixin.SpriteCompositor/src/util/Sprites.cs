using System.Collections.Generic;
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
        public static List<Sprite> RepeatRotated(Sprite spriteToRepeat, int repetitions, Anchor rotationAnchor) =>
            RepeatRotated(spriteToRepeat, repetitions, rotationAnchor, Angle.Full / repetitions);

        /// <summary>
        /// Clones a sprite a given number of times, rotating each clone with the given angle compared to the previous clone.
        /// </summary>
        /// <param name="spriteToRepeat">The sprite to clone</param>
        /// <param name="repetitions">Number of instances of the sprite there need to be</param>
        /// <param name="rotationAnchor">The center of rotation</param>
        /// <param name="stepSize">The angle to rotate each clone compared to the previous one</param>
        /// <returns>The resulting list of sprites, which includes the original sprite (though no operation has been applied to that sprite)</returns>
        public static List<Sprite> RepeatRotated(Sprite spriteToRepeat, int repetitions, Anchor rotationAnchor, Angle stepSize)
        {
            List<Sprite> resultingSprites = new List<Sprite>(repetitions) { spriteToRepeat };
            for (var i = 1; i < repetitions; i++)
            {
                var newSprite = spriteToRepeat.Clone();
                newSprite.Rotate(Angle.FromRadians(stepSize.AsRadians() * i), rotationAnchor);
                resultingSprites.Add(newSprite);
            }

            return resultingSprites;
        }

        public static TextureSprite Mirror(TextureSprite sprite)
        {
            sprite.Scale(new Vector2(-1, -1));
            return sprite;
        }

        public static SpriteGroup Mirror(SpriteGroup spriteGroup)
        {
            spriteGroup.Scale(new Vector2(-1, -1));
            return spriteGroup;
        }

        private static TextureSprite MirrorHorizontal(TextureSprite sprite)
        {
            sprite.Scale(new Vector2(-1, 1));
            return sprite;
        }

        private static SpriteGroup MirrorHorizontal(SpriteGroup spriteGroup)
        {
            spriteGroup.Scale(new Vector2(-1, 1));
            return spriteGroup;
        }

        private static TextureSprite MirrorVertical(TextureSprite sprite)
        {
            sprite.Scale(new Vector2(1, -1));
            return sprite;
        }

        private static SpriteGroup MirrorVertical(SpriteGroup spriteGroup)
        {
            spriteGroup.Scale(new Vector2(1, -1));
            return spriteGroup;
        }
    }
}