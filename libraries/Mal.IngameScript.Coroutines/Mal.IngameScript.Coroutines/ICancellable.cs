using System;

namespace IngameScript
{
    public interface ICancellable
    {
        bool IsCancellationRequested { get; }
        void WhenCancelled(Action action);
        void Cancel();
    }
}