using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public static class HashSetExtensions
    {
        public static bool ContainsAll<T>(this HashSet<T> set, IEnumerable<T> items) => items.All(set.Contains);
    }
}