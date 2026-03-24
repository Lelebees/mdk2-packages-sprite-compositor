using System.Collections.Generic;
using System.Linq;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class SpriteGroup : Sprite
    {
        private readonly List<Sprite> children;

        public SpriteGroup(List<Sprite> children)
        {
            this.children = children;
        }

        public Vector2 GetPosition()
        {
            //TODO: implement this
            return new Vector2(0, 0);
        }

        public void Translate(Vector2 vector)
        {
            children.ForEach(sprite => sprite.Translate(vector));
        }

        public void SetColor(Color color)
        {
            children.ForEach(sprite => sprite.SetColor(color));
        }

        public void SetAlignment(TextAlignment alignment)
        {
            children.ForEach(sprite => sprite.SetAlignment(alignment));
        }

        public void Scale(float amount)
        {
            //TODO: Provide better implementation
            children.ForEach(sprite => sprite.Scale(amount));
        }

        public void Rotate(Angle angle, Anchor anchor = null)
        {
            if (anchor == null) anchor = this;
            children.ForEach(sprite => sprite.Rotate(angle, anchor));
        }

        public Sprite Clone()
        {
            return new SpriteGroup(children.Select(sprite => sprite.Clone()).ToList());
        }

        public List<MySprite> ToDrawableList(RectangleF viewport)
        {
            return children.SelectMany(child => child.ToDrawableList(viewport)).ToList();
        }
    }
}