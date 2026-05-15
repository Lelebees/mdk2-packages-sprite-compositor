using FakeItEasy;
using IngameScript;
using NUnit.Framework;

namespace SpriteCompositor.Test
{
    [TestFixture]
    public class SpriteGroupTest
    {
        [Test]
        public void RotateShouldPassNonNullAnchorToChildren()
        {
            var child1 = A.Fake<Sprite>();
            var child2 = A.Fake<Sprite>();

            Anchor capturedAnchor1 = null;
            Anchor capturedAnchor2 = null;

            A.CallTo(() => child1.Rotate(A<Angle>._, A<Anchor>._))
                .Invokes((Angle _, Anchor anchor) => capturedAnchor1 = anchor);

            A.CallTo(() => child2.Rotate(A<Angle>._, A<Anchor>._))
                .Invokes((Angle _, Anchor anchor) => capturedAnchor2 = anchor);

            var group = Sprites.Group(child1, child2);

            
            group.Rotate(Angle.Right);
            
            Assert.That(capturedAnchor1, Is.Not.Null);
            Assert.That(capturedAnchor2, Is.Not.Null);

            A.CallTo(() => child1.Rotate(A<Angle>._, A<Anchor>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => child2.Rotate(A<Angle>._, A<Anchor>._))
                .MustHaveHappenedOnceExactly();
        }
        
        [Test]
        public void RotateShouldNotPassSelfAsAnchor()
        {
            var child1 = A.Fake<Sprite>();
            var child2 = A.Fake<Sprite>();

            Anchor capturedAnchor1 = null;
            Anchor capturedAnchor2 = null;

            A.CallTo(() => child1.Rotate(A<Angle>._, A<Anchor>._))
                .Invokes((Angle _, Anchor anchor) => capturedAnchor1 = anchor);

            A.CallTo(() => child2.Rotate(A<Angle>._, A<Anchor>._))
                .Invokes((Angle _, Anchor anchor) => capturedAnchor2 = anchor);

            var group = Sprites.Group(child1, child2);

            
            group.Rotate(Angle.Right);
            
            Assert.That(capturedAnchor1, Is.Not.EqualTo(group));
            Assert.That(capturedAnchor2, Is.Not.EqualTo(group));

            A.CallTo(() => child1.Rotate(A<Angle>._, A<Anchor>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => child2.Rotate(A<Angle>._, A<Anchor>._))
                .MustHaveHappenedOnceExactly();
        }
    }
}