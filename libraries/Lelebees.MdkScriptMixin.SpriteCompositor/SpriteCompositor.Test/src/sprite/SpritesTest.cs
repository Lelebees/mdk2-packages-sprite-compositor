using IngameScript;
using NUnit.Framework;

namespace SpriteCompositor.Test.util
{
    [TestFixture]
    public class SpritesTest
    {
        [TestCase((uint) 10)]
        [TestCase((uint) 1)]
        [TestCase((uint) 0)]
        public void SpritesRepeatCorrectAmountOfTimes(uint repetitions)
        {
            Assert.That(Sprites.RepeatRotated(Sprites.WithTexture("SquareSimple").Build(), repetitions).Count, Is.EqualTo((int) repetitions));
        }
        
        [TestCase((uint) 10)]
        [TestCase((uint) 1)]
        [TestCase((uint) 0)]
        public void SpritesWithRotationRepeatCorrectAmountOfTimes(uint repetitions)
        {
            Assert.That(Sprites.RepeatRotated(Sprites.WithTexture("SquareSimple").Build(), repetitions, Angle.Right).Count, Is.EqualTo((int) repetitions));
        }
    }
}