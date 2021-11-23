using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sysx.Linq
{
    /// <summary>
    /// Memoizes the enumerable, ensuring that it will only be evaluated up to one time, even if it is called more than once.
    /// </summary>
    public class MemoizedEnumerable<T> : IEnumerable<T>
    {
        private readonly object locker = new { };
        private IEnumerable<T>? cache;
        private readonly IEnumerable<T> source;

        public MemoizedEnumerable(IEnumerable<T> enumerable)
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

                cache = source.ToArray();
            }
        }
    }
}
