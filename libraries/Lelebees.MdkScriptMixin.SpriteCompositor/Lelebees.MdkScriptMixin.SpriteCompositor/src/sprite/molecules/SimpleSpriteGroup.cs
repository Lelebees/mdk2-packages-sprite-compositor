using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public class SimpleSpriteGroup : SpriteGroup
    {
        private readonly List<Sprite> children;
        
        public SimpleSpriteGroup(List<Sprite> children)
        {
            this.children = children;
        }
        
        public override Sprite Clone()
        {
            return new SimpleSpriteGroup(children.Select(sprite => sprite.Clone()).ToList());
        }

        protected override List<Sprite> GetChildren()
        {
            return children;
        }
    }
}