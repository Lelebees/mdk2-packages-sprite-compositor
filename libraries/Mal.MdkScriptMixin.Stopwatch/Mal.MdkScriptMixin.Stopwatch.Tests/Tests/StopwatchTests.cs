using System;
using FakeItEasy;
using IngameScript;
using Mal.MdkScriptMixin.Stopwatch.Tests.TestUtilities;
using NUnit.Framework;
using Sandbox.ModAPI.Ingame;

namespace Mal.MdkScriptMixin.Stopwatch.Tests.Tests
{
    using Stopwatch = IngameScript.Stopwatch;

    /// <summary>
    ///     Unit tests for the <see cref="Stopwatch" /> class.
    /// </summary>
    [TestFixture]
    public class StopwatchTests
    {
        [SetUp]
        public void SetUp()
        {
            _runtime = A.Fake<IMyGridProgramRuntimeInfo>();
            _program = Gateway.CreateProgram<Program>()
                .WithRuntime(_runtime)
                .Build();
        }

        Program _program;
        IMyGridProgramRuntimeInfo _runtime;

        [Test]
        public void NewStopwatch_WhenCreated_IsNotRunning()
        {
            // Act
            var stopwatch = new Stopwatch(_program);

            // Assert
            Assert.That(stopwatch.IsRunning, Is.False);
        }

        [Test]
        public void NewStopwatch_WhenCreated_HasZeroElapsedTicks()
        {
            // Act
            var stopwatch = new Stopwatch(_program);

            // Assert
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(0));
        }

        [Test]
        public void Start_WhenCalled_SetsIsRunningToTrue()
        {
            // Arrange
            var stopwatch = new Stopwatch(_program);

            // Act
            stopwatch.Start();

            // Assert
            Assert.That(stopwatch.IsRunning, Is.True);
        }

        [Test]
        public void ElapsedTicks_WhenRunning_ReturnsCorrectElapsedTime()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(100);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();

            // Act
            A.CallTo(() => _runtime.LifetimeTicks).Returns(150);

            // Assert
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(50));
        }

        [Test]
        public void Stop_WhenRunning_SetsIsRunningToFalse()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(100);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();

            // Act
            A.CallTo(() => _runtime.LifetimeTicks).Returns(150);
            stopwatch.Stop();

            // Assert
            Assert.That(stopwatch.IsRunning, Is.False);
        }

        [Test]
        public void Stop_WhenRunning_FreezesElapsedTicks()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(100);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();

            // Act
            A.CallTo(() => _runtime.LifetimeTicks).Returns(150);
            stopwatch.Stop();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(200); // Time passes while stopped

            // Assert
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(50)); // Should still be 50, not 100
        }

        [Test]
        public void Stop_WhenNotRunning_DoesNothing()
        {
            // Arrange
            var stopwatch = new Stopwatch(_program);

            // Act
            stopwatch.Stop();

            // Assert
            Assert.That(stopwatch.IsRunning, Is.False);
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(0));
        }

        [Test]
        public void StartStopStart_WhenCalled_AccumulatesTimeCorrectly()
        {
            // Arrange
            var stopwatch = new Stopwatch(_program);

            // First session: run for 50 ticks
            A.CallTo(() => _runtime.LifetimeTicks).Returns(0);
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(50);
            stopwatch.Stop();

            // Time passes while stopped (should be ignored)
            A.CallTo(() => _runtime.LifetimeTicks).Returns(100);

            // Second session: run for 30 ticks
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(130);

            // Assert
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(80)); // 50 + 30
        }

        [Test]
        public void StartStopMultipleTimes_AccumulatesTimeCorrectly()
        {
            // Arrange
            var stopwatch = new Stopwatch(_program);

            // Session 1: 0 -> 4 (4 ticks)
            A.CallTo(() => _runtime.LifetimeTicks).Returns(0);
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(4);
            stopwatch.Stop();
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(4));

            // Stopped for 4 ticks (4 -> 8)
            A.CallTo(() => _runtime.LifetimeTicks).Returns(8);

            // Session 2: 8 -> 12 (4 ticks)
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(12);
            stopwatch.Stop();
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(8));

            // Stopped for 4 ticks (12 -> 16)
            A.CallTo(() => _runtime.LifetimeTicks).Returns(16);

            // Session 3: 16 -> 20 (4 ticks)
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(20);
            stopwatch.Stop();

            // Assert
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(12)); // 4 + 4 + 4
        }

        [Test]
        public void Start_WhenAlreadyRunning_DoesNotResetStartTime()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(0);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();

            A.CallTo(() => _runtime.LifetimeTicks).Returns(50);

            // Act - Call Start again while already running
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(100);

            // Assert - Should have 100 ticks elapsed, not 50
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(100));
        }

        [Test]
        public void Restart_WhenCalled_ResetsElapsedTicks()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(0);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(100);

            // Act
            stopwatch.Restart();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(150);

            // Assert
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(50)); // Not 150
        }

        [Test]
        public void Restart_WhenCalled_SetsIsRunningToTrue()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(0);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();
            stopwatch.Stop();

            // Act
            stopwatch.Restart();

            // Assert
            Assert.That(stopwatch.IsRunning, Is.True);
        }

        [Test]
        public void Reset_WhenCalled_ClearsElapsedTicks()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(0);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(100);
            stopwatch.Stop();

            // Act
            stopwatch.Reset();

            // Assert
            Assert.That(stopwatch.ElapsedTicks, Is.EqualTo(0));
        }

        [Test]
        public void Reset_WhenCalled_SetsIsRunningToFalse()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(0);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();

            // Act
            stopwatch.Reset();

            // Assert
            Assert.That(stopwatch.IsRunning, Is.False);
        }

        [Test]
        public void Elapsed_WithZeroTicks_ReturnsZeroTimeSpan()
        {
            // Arrange
            var stopwatch = new Stopwatch(_program);

            // Act
            var elapsed = stopwatch.Elapsed;

            // Assert
            Assert.That(elapsed, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void Elapsed_With60Ticks_ReturnsOneSecond()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(0);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(60);

            // Act
            var elapsed = stopwatch.Elapsed;

            // Assert
            Assert.That(elapsed, Is.EqualTo(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void Elapsed_With3600Ticks_ReturnsOneMinute()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(0);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(3600);

            // Act
            var elapsed = stopwatch.Elapsed;

            // Assert
            Assert.That(elapsed, Is.EqualTo(TimeSpan.FromMinutes(1)));
        }

        [Test]
        public void ToString_ReturnsElapsedTimeAsString()
        {
            // Arrange
            A.CallTo(() => _runtime.LifetimeTicks).Returns(0);
            var stopwatch = new Stopwatch(_program);
            stopwatch.Start();
            A.CallTo(() => _runtime.LifetimeTicks).Returns(60);

            // Act
            var result = stopwatch.ToString();

            // Assert
            Assert.That(result, Is.EqualTo(TimeSpan.FromSeconds(1).ToString()));
        }
    }
}