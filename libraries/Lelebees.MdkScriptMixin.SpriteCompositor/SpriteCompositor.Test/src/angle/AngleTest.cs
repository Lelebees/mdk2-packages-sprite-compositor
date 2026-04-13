using System;
using IngameScript;
using NUnit.Framework;

namespace SpriteCompositor.Test.angle
{
    [TestFixture]
    public class AngleTest
    {
        private const double Precision = 0.000000000000000000000000001;

        [TestCase(360, 2 * Math.PI)]
        [TestCase(90, 0.5 * Math.PI)]
        [TestCase(180, Math.PI)]
        [TestCase(270, 1.5 * Math.PI)]
        [TestCase(1800, 0)]
        [TestCase(0, 0)]
        public void DegreesConvertProperly(double degrees, double radians)
        {
            Assert.That(Angle.FromDegrees(degrees).AsRadians(), Is.InRange(radians - Precision, radians + Precision));
        }
    }
}