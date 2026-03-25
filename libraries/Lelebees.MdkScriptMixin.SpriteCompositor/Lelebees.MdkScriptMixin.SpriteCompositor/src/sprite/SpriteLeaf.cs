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

        public void Translate(float x, float y)
        {
            Translate(new Vector2(x,y));
        }

        public void SetColor(Color color)
        {
            Sprite.Color = color;
        }

        public void SetAlignment(TextAlignment alignment)
        {
            Sprite.Alignment = alignment;
        }

        public abstract void Scale(float scalar, Anchor anchor = null);
        public abstract void Scale(Vector2 scalar, Anchor anchor = null);

        public abstract void Rotate(Angle angle, Anchor anchor = null);
        public abstract Sprite Clone();
        public MySprite[] AsDrawableCollection(RectangleF viewport)
        {
            var copy = new MySprite(
                Sprite.Type,
                Sprite.Data,
                Sprite.Position + viewport.Center,
                Sprite.Size,
                Sprite.Color,
                Sprite.FontId,
                Sprite.Alignment,
                Sprite.RotationOrScale);
            return new[] { copy };
        }

        public MySprite[] AsDrawableCollection()
        {
            var copy = new MySprite(
                Sprite.Type,
                Sprite.Data,
                Sprite.Position,
                Sprite.Size,
                Sprite.Color,
                Sprite.FontId,
                Sprite.Alignment,
                Sprite.RotationOrScale);
            return new[] { copy };
        }
    }
}