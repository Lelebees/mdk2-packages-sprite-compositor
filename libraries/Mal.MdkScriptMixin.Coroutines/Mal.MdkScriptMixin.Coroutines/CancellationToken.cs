using System;

namespace IngameScript
{
    public struct CancellationToken
    {
        public static readonly CancellationToken Default = new CancellationToken();

        readonly ICancellable _source;
        readonly int _version;

        public CancellationToken(ICancellable source, int version)
        {
            _source = source;
            _version = version;
        }

        public int Version => _version;
        public bool IsCancellationRequested => _source != null && _source.Version != _version;
        public void WhenCancelled(Action action) => _source?.WhenCancelled(action);
    }
}