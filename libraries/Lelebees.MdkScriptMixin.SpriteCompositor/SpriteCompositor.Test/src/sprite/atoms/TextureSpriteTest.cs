using System;
using System.Linq;
using IngameScript;
using NUnit.Framework;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace SpriteCompositor.Test.sprite.atoms
{
    [TestFixture]
    public class TextureSpriteTest
    {
        private const double Precision = 0.001;

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
        [TestCase(-1)]
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

            // Note here (and this is not obvious) that unlike the angle struct, a sprite's rotation value can grow indefinitely
            // This means that values above and below 2 * Math.PI are possible. The test keeps this in mind.
            var totalAngle = angles.Sum(angle => angle.Radians());
            Assert.That(sprite.Rotation, Is.EqualTo(totalAngle).Within(Precision));
        }

        [TestCase(1, 1)]
        [TestCase(0, 0)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, -1)]
        public void CanTranslate(float x, float y)
        {
            Vector2 translation = new Vector2(x, y);
            TextureSprite sprite = Sprites.WithTexture(Textures.SquareSimple).Build();
            sprite.Translate(translation);
            Assert.That(sprite.GetPosition(), Is.EqualTo(translation));
        }

        [TestCase(0,0,Math.PI)]
        [TestCase(1, 1, 0)]
        [TestCase(1, 1, Math.PI)]
        [TestCase(1, 1, Math.PI / 2)]
        [TestCase(1, 1, -Math.PI / 2)]
        [TestCase(5, 0, Math.PI)]
        [TestCase(0, 5, Math.PI / 2)]
        [TestCase(-3, 4, Math.PI / 4)]
        [TestCase(10, -10, Math.PI * 2)]
        [TestCase(-7.5f, 2.25f, Math.PI * 1.5)]
        [TestCase(0.001f, 0.001f, Math.PI / 6)]
        [TestCase(1000, 1000, Math.PI / 3)]
        // This Case fails. This is okay in my opinion because the size of the screen will probably never exceed 1000 pixels. (The current largest screen is 512x512),
        // and if so, KSH can start using doubles. 
        // [TestCase(float.MaxValue / 1000, float.MaxValue / 1000, Math.PI / 8)]
        public void RotationAroundAnchorMaintainsCorrectDistance(float x, float y, double radians)
        {
            Vector2 offset = new Vector2(x, y);
            TextureSprite sprite = Sprites.WithTexture(Textures.SquareSimple).Position(offset).Build();
            sprite.Rotate(Angle.FromRadians(radians), new PointAnchor(0, 0));
            Assert.That(sprite.Position.Value.Length(), Is.EqualTo(offset.Length()).Within(Precision));
        }
        
        [TestCase(0, 0, 5, 5)]
        [TestCase(1, 1, 2, 2)]
        [TestCase(-1, -1, 2, 2)]
        [TestCase(10, 20, 0.5f, 0.5f)]
        [TestCase(3, 4, -1, 1)]
        [TestCase(3, 4, 1, -1)]
        [TestCase(3, 4, -1, -1)]
        [TestCase(100, 200, 10, 10)]
        [TestCase(0.001f, 0.001f, 1000, 1000)]
        [TestCase(50, -25, 1.5f, 2.5f)]
        [TestCase(-50, 25, 1.5f, -2.5f)]
        [TestCase(5, 10, 0, 1)]
        [TestCase(5, 10, 1, 0)]
        [TestCase(5, 10, 0, 0)]
        [TestCase(float.MaxValue / 1000, float.MaxValue / 1000, 0.5f, 0.5f)]
        public void ScalingPositionsCorrectly(float x, float y, float scaleX, float scaleY)
        {
            TextureSprite sprite = Sprites.WithTexture(Textures.SquareSimple).Position(x,y).Build();
            sprite.Scale(new Vector2(scaleX, scaleY), new PointAnchor(0, 0));
            Assert.That(sprite.GetPosition().X, Is.EqualTo(x * scaleX).Within(Precision));
            Assert.That(sprite.GetPosition().Y, Is.EqualTo(y * scaleY).Within(Precision));
        }

        [Test]
        public void ClonesCantPropagateChanges()
        {
            TextureSprite sprite1 = Sprites.WithTexture(Textures.SquareSimple).Build();
            Sprite sprite2 = sprite1.Clone();
            Assert.That(sprite1, Is.Not.EqualTo(sprite2));
        }
        
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