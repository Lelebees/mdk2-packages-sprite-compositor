using System;
using System.Linq;
using IngameScript;
using NUnit.Framework;

namespace SpriteCompositor.Test.sprite.atoms
{
    [TestFixture]
    public class TextureSpriteTest
    {
        // KSH uses floats for the angles, which is why the precision is so low.
        private const double Precision = 0.000001;

        [TestCase(0.5 * Math.PI)]
        [TestCase(Math.PI)]
        [TestCase(0.25 * Math.PI)]
        [TestCase(0.75 * Math.PI)]
        [TestCase(1.2 * Math.PI)]
        [TestCase(0.23 * Math.PI)]
        [TestCase(1.93 * Math.PI)]
        [TestCase(43 * Math.PI)]
        [TestCase(0)]
        [TestCase(-1 * Math.PI)]
        [TestCase (-1)]
        [TestCase(1)]
        [TestCase(1, 69, 420)]
        [TestCase(0.25 * Math.PI, 0.75 * Math.PI)]
        public void ArbitraryRotationsSetCorrectRotation(params double[] radians)
        {
            Angle[] angles = radians.Select(Angle.FromRadians).ToArray();
            TextureSprite sprite = Sprites.WithTexture(Textures.SquareSimple).Build();
            foreach (var angle in angles)
            {
                sprite.Rotate(angle);    
            }
            // Note here (and this is not obvious) that unlike the angle struct, a sprite's rotation value will grow indefinitely
            // This means that values above and below 2 * Math.PI are possible. The test keeps this in mind.
            var totalAngle = angles.Sum(angle => angle.AsRadians());
            Assert.That(sprite.Rotation, Is.InRange( totalAngle - Precision, totalAngle + Precision));
        }
    }
}