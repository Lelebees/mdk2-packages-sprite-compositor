using System;

namespace IngameScript
{
    public class Cancellation : ICancellable
    {
        public Cancellation()
        {
            Version = 0;
            Token = new CancellationToken(this, Version);
        }

        public CancellationToken Token { get; private set; }

        public void WhenCancelled(Action action)
        {
            CancelRequested += c => action();
        }

        public int Version { get; private set; }

        public void Cancel()
        {
            Version++;
            Token = new CancellationToken(this, Version);
            CancelRequested?.Invoke(this);
        }

        public event Action<ICancellable> CancelRequested;

        public static CancellationToken Combine(CancellationToken token1, CancellationToken token2)
        {
            return new CancellationToken(new Combination(token1, token2), 0);
        }

        class Combination : ICancellable
        {
            CancellationToken _token1, _token2;

            public Combination(CancellationToken token1, CancellationToken token2)
            {
                _token1 = token1;
                _token2 = token2;
                Version = 0;
            }

            public void WhenCancelled(Action action)
            {
                _token1.WhenCancelled(action);
                _token2.WhenCancelled(action);
            }

            public int Version { get; }

            public void Cancel()
            {
                throw new Exception("Cannot cancel a combined token directly.");
            }
        }
    }
}