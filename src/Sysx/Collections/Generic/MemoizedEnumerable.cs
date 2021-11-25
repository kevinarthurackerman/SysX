using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if NET5_0 || NETCOREAPP3_1
using System.Collections.Immutable;
#endif

namespace Sysx.Collections.Generic
{
    /// <summary>
    /// Memoizes the enumerable, ensuring that it will only be evaluated up to one time, even if it is called more than once.
    /// </summary>
    public class MemoizedEnumerable<T> : IEnumerable<T>
    {
        private readonly object locker = new { };
        private IEnumerable<T>? cache;
        private IEnumerable<T>? source;

        public static IEnumerable<T>? Create(IEnumerable<T>? enumerable)
        {
            if (enumerable == null) return null;
            if (enumerable is T[] array) return array;
#if NET5_0 || NETCOREAPP3_1
            if (enumerable is ImmutableArray<T> immutableArray) return immutableArray;
            if (enumerable is IImmutableList<T> immutableList) return immutableList;
            if (enumerable is IImmutableQueue<T> immutableQueue) return immutableQueue;
            if (enumerable is IImmutableSet<T> immutableSet) return immutableSet;
            if (enumerable is IImmutableStack<T> immutableStack) return immutableStack;
#endif

            return new MemoizedEnumerable<T>(enumerable);
        }

        private MemoizedEnumerable(IEnumerable<T> enumerable)
        {
            source = enumerable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Initialize();
            return cache!.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Initialize();
            return ((IEnumerable)cache!).GetEnumerator();
        }

        private void Initialize()
        {
            if (cache != null) return;

            lock (locker)
            {
                if (cache != null) return;

                cache = source!.ToArray();
                source = null;
            }

            Assert.That(cache == null ^ source == null);
        }
    }
}
