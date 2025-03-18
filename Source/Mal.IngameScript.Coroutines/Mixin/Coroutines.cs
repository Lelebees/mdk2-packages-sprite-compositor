using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

// ReSharper disable InconsistentNaming

namespace IngameScript
{
    public class Coroutines
    {
        static ulong __idSrc;
        readonly Program _program;
        readonly List<Coroutine> _r = new List<Coroutine>();
        readonly HashSet<ulong> _rIds = new HashSet<ulong>();

        internal Coroutines(Program program)
        {
            _program = program;
        }

        public int Count => _r.Count;

        public int Main(string argument, UpdateType updateSource)
        {
            var i = 0;
            while (i < _r.Count)
            {
                var c = _r[i];
                if (c.W.U == 0 || (updateSource & c.W.U) != 0)
                {
                    if ((c.W.F != 1 || !_rIds.Contains(c.W.B)) && c.W.C?.Invoke(c.W) != false)
                    {
                        if (!c.R.MoveNext())
                        {
                            _r.RemoveAtFast(i);
                            _rIds.Remove(c.Id);
                            continue;
                        }

                        _r[i] = new Coroutine { Id = c.Id, R = c.R, W = c.R.Current };
                    }
                }

                i++;
            }

            var f = UpdateFrequency.None;
            for (var j = 0; j < _r.Count; j++) f |= _r[j].W.GetUpdateFrequency();
            _program.Runtime.UpdateFrequency = f;
            return i;
        }

        public ulong Run(IEnumerator<When> coroutine)
        {
            var c = new Coroutine { Id = ++__idSrc, R = coroutine, W = new When(CrFrequency.Immediate) };
            _r.Add(c);
            _rIds.Add(c.Id);
            _program.Runtime.UpdateFrequency |= c.W.GetUpdateFrequency();
            return c.Id;
        }

        public ulong Run(CoroutineFn coroutineFn, CancellationToken token = default(CancellationToken)) => Run(coroutineFn(token));

        public bool IsCompleted(ulong id) => !_rIds.Contains(id);

        struct Coroutine
        {
            public ulong Id;
            public IEnumerator<When> R;
            public When W;
        }
    }
}