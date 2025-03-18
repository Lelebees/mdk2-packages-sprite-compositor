using System;

namespace IngameScript
{
    public class Cancellation : ICancellable
    {
        public Cancellation()
        {
            Token = new CancellationToken(this);
        }

        public CancellationToken Token { get; }

        public void WhenCancelled(Action action)
        {
            if (IsCancellationRequested)
                action();
            else
                CancelRequested += c => action();
        }

        public bool IsCancellationRequested { get; private set; }

        public void Cancel()
        {
            if (IsCancellationRequested)
                return;
            IsCancellationRequested = true;
            CancelRequested?.Invoke(this);
        }

        public void Reset() => IsCancellationRequested = false;

        public event Action<ICancellable> CancelRequested;

        public static CancellationToken Combine(CancellationToken token1, CancellationToken token2) => new CancellationToken(new Combination(token1, token2));

        class Combination : ICancellable
        {
            CancellationToken _token1, _token2;

            public Combination(CancellationToken token1, CancellationToken token2)
            {
                _token1 = token1;
                _token2 = token2;
            }

            public void WhenCancelled(Action action)
            {
                if (IsCancellationRequested)
                    action();
                else
                {
                    _token1.WhenCancelled(action);
                    _token2.WhenCancelled(action);
                }
            }

            public bool IsCancellationRequested => _token1.IsCancellationRequested || _token2.IsCancellationRequested;

            public void Cancel() { }
        }
    }
}