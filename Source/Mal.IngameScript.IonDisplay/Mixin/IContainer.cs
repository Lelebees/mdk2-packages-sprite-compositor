using System.Collections.Generic;

namespace IngameScript
{
    public interface IContainer : IView
    {
        void Add(View view);
        void AddRange(IEnumerable<View> views);
    }
}