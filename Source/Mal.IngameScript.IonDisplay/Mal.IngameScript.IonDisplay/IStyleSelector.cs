using System;

namespace IngameScript
{
    public interface IStyleSelector
    {
        Type Type { get; }
        IStyleSelector Parent { get; }
        bool Matches(View view);
    }
}