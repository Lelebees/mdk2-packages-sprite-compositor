using IngameScript;
using NUnit.Framework;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace SpriteCompositor.Test
{
    [TestFixture]
    public class CompositeSpriteTest
    {
        [Test]
        public void RenderableDoesNotPropagateChanges()
        {
            var sprite = Sprites.Compose(Sprites.WithText("wow").Build());
            var mySprite = sprite.AsDrawableCollection()[0];
            mySprite.Alignment = TextAlignment.RIGHT;
            mySprite.Color = Color.Red;
            mySprite.Data = "Circle";
            mySprite.RotationOrScale = 99f;
            var newDrawable = sprite.AsDrawableCollection()[0];
            Assert.That(newDrawable, Is.Not.EqualTo(mySprite));
        }

        [Test]
        public void RenderableWithViewportDoesNotChangeSprite()
        {
            var sprite = Sprites.Compose(Sprites.WithText("uau").Build());
            var nonProblematicDrawable = sprite.AsDrawableCollection()[0];
            sprite.AsDrawableCollection(new RectangleF(10, 10, 20, 20));
            var potentialProblem = sprite.AsDrawableCollection()[0];
            Assert.That(nonProblematicDrawable, Is.EqualTo(potentialProblem));
        }
    }
}