using System;

namespace IngameScript
{
    public interface ICancellable
    {
        void WhenCancelled(Action action);
        void Cancel();
        int Version { get; }
    }
}