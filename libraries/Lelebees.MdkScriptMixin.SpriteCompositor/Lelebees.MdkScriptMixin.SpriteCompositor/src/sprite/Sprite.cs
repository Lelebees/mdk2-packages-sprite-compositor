using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public interface Sprite : Anchor
    {
        /// <summary>
        /// Moves a sprite in 2D space.
        /// </summary>
        /// <param name="vector">How far the sprite will be moved and in what direction</param>
        void Translate(Vector2 vector);
        void SetColor(Color color);
        void SetAlignment(TextAlignment alignment);
        void Scale(float amount);
        /// <summary>
        /// Rotates a sprite in clockwise direction around the given anchor
        /// </summary>
        /// <param name="angle">The angle of rotation</param>
        /// <param name="anchor">The point in space to rotate around, if no anchor is given, the center of this sprite is used</param>
        void Rotate(Angle angle, Anchor anchor = null);
        
        /// <summary>
        /// Creates a copy of the sprite that can be modified separately.
        /// </summary>
        /// <returns>A new instance of the sprite</returns>
        Sprite Clone();

        List<MySprite> ToDrawableList(RectangleF viewport);
    }
}