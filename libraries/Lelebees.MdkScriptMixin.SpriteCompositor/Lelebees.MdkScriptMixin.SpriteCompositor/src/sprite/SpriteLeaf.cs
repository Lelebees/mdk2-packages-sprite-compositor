using System;
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
            Translate(new Vector2(x, y));
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

        public virtual void Scale(Vector2 scalar, Anchor anchor = null)
        {
            if (anchor == null || anchor == this || anchor.GetPosition() == GetPosition()) return;

            var anchorPos = anchor.GetPosition();
            var distanceFromAnchor = GetPosition() - anchorPos;
            distanceFromAnchor *= scalar;
            Sprite.Position = anchorPos + distanceFromAnchor;
        }

        public virtual void Rotate(Angle angle, Anchor positionAnchor = null)
        {
            if (positionAnchor == null || positionAnchor == this ||
                positionAnchor.GetPosition() == GetPosition()) return;

            var cos = Math.Cos(-angle.AsRadians());
            var sin = Math.Sin(-angle.AsRadians());

            var anchor = positionAnchor.GetPosition();

            var position = Sprite.Position ?? Vector2.Zero;
            var posX = (float)(cos * (position.X - anchor.X) + sin * (position.Y - anchor.Y) + anchor.X);
            var posY = (float)(-sin * (position.X - anchor.X) + cos * (position.Y - anchor.Y) + anchor.Y);
            Sprite.Position = new Vector2(posX, posY);
        }

        public abstract Sprite Clone();

        public MySprite[] AsDrawableCollection(RectangleF viewport)
        {
            return new[]
            {
                new MySprite(
                    Sprite.Type,
                    Sprite.Data,
                    Sprite.Position + viewport.Center,
                    Sprite.Size,
                    Sprite.Color,
                    Sprite.FontId,
                    Sprite.Alignment,
                    Sprite.RotationOrScale)
            };
        }

        public MySprite[] AsDrawableCollection()
        {
            return new[] { Sprite };
        }
    }
}