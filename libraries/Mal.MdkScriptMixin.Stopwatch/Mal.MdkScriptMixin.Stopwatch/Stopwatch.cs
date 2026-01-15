using System;

namespace IngameScript
{
    public class Stopwatch
    {
        readonly Program _program;
        long _startTicks, _elapsedTicks;

        public Stopwatch(Program program)
        {
            _program = program;
        }

        public bool IsRunning { get; private set; }

        public long ElapsedTicks => IsRunning
            ? _elapsedTicks + (_program.Runtime.LifetimeTicks - _startTicks)
            : _elapsedTicks;

        public TimeSpan Elapsed => TimeSpan.FromMilliseconds(ElapsedTicks * (1000.0 / 60.0));

        public Stopwatch Start()
        {
            if (!IsRunning)
            {
                _startTicks = _program.Runtime.LifetimeTicks;
                IsRunning = true;
            }

            return this;
        }

        public Stopwatch Restart()
        {
            _startTicks = _program.Runtime.LifetimeTicks;
            _elapsedTicks = 0;
            IsRunning = true;
            return this;
        }

        public Stopwatch Stop()
        {
            if (IsRunning)
            {
                _elapsedTicks += _program.Runtime.LifetimeTicks - _startTicks;
                IsRunning = false;
            }

            return this;
        }

        public Stopwatch Reset()
        {
            _startTicks = 0;
            _elapsedTicks = 0;
            IsRunning = false;
            return this;
        }

        public override string ToString()
        {
            return Elapsed.ToString();
        }
    }
}