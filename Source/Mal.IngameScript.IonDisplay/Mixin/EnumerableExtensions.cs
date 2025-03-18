using System.Collections.Generic;

namespace IngameScript
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> InjectBefore<T>(this IEnumerable<T> source, T value)
        {
            yield return value;
            foreach (var item in source)
                yield return item;
        }

        public static IEnumerable<T> InjectAfter<T>(this IEnumerable<T> source, T value)
        {
            foreach (var item in source)
                yield return item;
            yield return value;
        }
    }
}