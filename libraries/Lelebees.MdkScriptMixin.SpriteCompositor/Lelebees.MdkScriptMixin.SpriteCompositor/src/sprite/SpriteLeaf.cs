using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public abstract class SpriteLeaf : Sprite
    {

        protected MySprite Sprite;

        protected SpriteLeaf(MySprite sprite)
        {
            Sprite = sprite;
        }

        public Vector2 GetPosition()
        {
            return Sprite.Position ?? new Vector2(0, 0);
        }

        public void Translate(Vector2 vector)
        {
            if (Sprite.Position == null)
            {
                Sprite.Position = new Vector2(0, 0);
            }

            var position = Sprite.Position.Value;
            position.X += vector.X;
            position.Y += vector.Y;
            Sprite.Position = position;
        }

        public void SetColor(Color color)
        {
            Sprite.Color = color;
        }

        public void SetAlignment(TextAlignment alignment)
        {
            Sprite.Alignment = alignment;
        }

        public abstract void Scale(float amount);

        public abstract void Rotate(Angle angle, Anchor anchor = null);
    }
}