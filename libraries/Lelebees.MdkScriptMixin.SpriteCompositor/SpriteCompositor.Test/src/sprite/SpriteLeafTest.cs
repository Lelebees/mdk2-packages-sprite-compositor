using IngameScript;
using NUnit.Framework;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace SpriteCompositor.Test.sprite
{
    [TestFixture]
    public class SpriteLeafTest
    {
        [Test]
        public void RenderableDoesNotPropagateChanges()
        {
            var sprite = Sprites.WithTexture("SquareSimple").Build();
            var mySprite = sprite.AsDrawableCollection()[0];
            mySprite.Alignment = TextAlignment.RIGHT;
            mySprite.Color = Color.Red;
            mySprite.Data = "Circle";
            mySprite.RotationOrScale = 99f;
            var newDrawable = sprite.AsDrawableCollection()[0];
            Assert.That(newDrawable, Is.Not.EqualTo(mySprite));
        }    
    }
}