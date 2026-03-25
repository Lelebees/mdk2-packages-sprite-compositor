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
        /// <summary>
        /// Moves a sprite in 2D space.
        /// </summary>
        /// <param name="x">How far the sprite will be moved on the x-axis</param>
        /// <param name="y">How far the sprite will be moved on the y-axis</param>
        void Translate(float x, float y);
        void SetColor(Color color);
        void SetAlignment(TextAlignment alignment);
        /// <summary>
        /// Scales a sprite in 2 dimensions according to the scalar.
        /// </summary>
        /// <param name="scalar">How much to multiply the size of the sprite by</param>
        /// <param name="anchor">If not null, will also scale the sprite's distance from the anchor</param>
        void Scale(float scalar, Anchor anchor = null);
        /// <summary>
        /// Scales a sprite in 2 dimensions according to the scalar.
        /// </summary>
        /// <param name="scalar">How much to multiply the size of the sprite by. If either x or y is 1, scales in only one dimension (except for text)</param>
        /// <param name="anchor">If not null, will also scale the sprite's distance from the anchor</param>
        void Scale(Vector2 scalar, Anchor anchor = null);
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

        /// <summary>
        /// Converts the sprite to a collection of sprites that Space Engineers can actually draw.
        /// </summary>
        /// <param name="viewport">The size of the screen the sprite(s) are to be drawn to. Moves the sprites so that 0,0 is the center of the screen.</param>
        /// <returns>A collection of MySprite objects that can be drawn on a screen the size of viewport.</returns>
        MySprite[] AsDrawableCollection(RectangleF viewport);
        /// <summary>
        /// Converts the sprite to a collection of sprites that Space Engineers can actually draw
        /// </summary>
        /// <returns>A collection of MySprite objects. These have not been adjusted to fit in the viewport.</returns>
        MySprite[] AsDrawableCollection();
    }
}