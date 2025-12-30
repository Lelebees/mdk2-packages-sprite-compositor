using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public static class CoroutinesForEach
    {
#if !MAL_SPACEENGINEERS_INGAMESCRIPT_OBJECTPOOL
        static readonly Queue<List<ulong>> ListPool = new Queue<List<ulong>>();
#endif
        public static void ForEach<T>(this Coroutines coroutines, IReadOnlyList<T> blocks, Action<T> action, UpdateType updateType = UpdateType.Update1, int batchSize = 25) => coroutines.Run(ForEachCoroutine(blocks, action, updateType, batchSize));

        public static void WhenAll<T>(this Coroutines coroutines, IEnumerable<ulong> coroutineIds, Action<T> action, UpdateType updateType = UpdateType.Update1, int batchSize = 25) => coroutines.Run(WhenAllCoroutine(coroutines, coroutineIds, action, updateType));

        static IEnumerator<When> WhenAllCoroutine<T>(Coroutines coroutines, IEnumerable<ulong> coroutineIds, Action<T> action, UpdateType updateType)
        {
#if MAL_SPACEENGINEERS_INGAMESCRIPT_OBJECTPOOL
            var list = ObjectPool.Get<List<ulong>>();
#else
            var list = ListPool.Count > 0 ? ListPool.Dequeue() : new List<ulong>();
#endif
            try
            {
                list.AddRange(coroutineIds);
                while (true)
                {
                    if (list.All(coroutines.IsCompleted))
                        break;
                    yield return When.Next(updateType);
                }
            }
            finally
            {
                list.Clear();
                ListPool.Enqueue(list);
            }
        }

        static IEnumerator<When> ForEachCoroutine<T>(IReadOnlyList<T> blocks, Action<T> action, UpdateType updateType, int batchSize)
        {
            for (var index = 0; index < blocks.Count; index++)
            {
                action(blocks[index]);
                if (index % batchSize == 0)
                    yield return When.Next(updateType);
            }
        }
    }
}