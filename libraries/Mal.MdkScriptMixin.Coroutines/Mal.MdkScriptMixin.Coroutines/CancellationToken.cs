using System;

namespace IngameScript
{
    public struct CancellationToken
    {
        public static readonly CancellationToken Default = new CancellationToken();

        readonly ICancellable _source;

        public CancellationToken(ICancellable source)
        {
            _source = source;
        }

        public bool IsCancellationRequested => _source != null && _source.IsCancellationRequested;
        public void WhenCancelled(Action action) => _source?.WhenCancelled(action);
    }
}