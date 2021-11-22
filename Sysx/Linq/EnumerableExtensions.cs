using System;
using System.Collections.Generic;
using System.Linq;

namespace Sysx.Linq
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Pair<T>> CrossApply<T>(this IEnumerable<T> enumerable) =>
            CrossApply(enumerable, enumerable, (left, right) => new Pair<T>(left, right));

        public static IEnumerable<Pair<T>> CrossApply<T>(this IEnumerable<T> left, IEnumerable<T> right) =>
            CrossApply(left, right, (left, right) => new Pair<T>(left, right));

        public static IEnumerable<Pair<TLeft, TRight>> CrossApply<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right) =>
            CrossApply(left, right, (left, right) => new Pair<TLeft, TRight>(left, right));

        public static IEnumerable<TResult> CrossApply<TLeft, TRight, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft,TRight,TResult> resultSelector)
        {
            return left.Join(right, x => true, x => true, (left, right) => (Left: left, Right: right))
                .Select(x => resultSelector(x.Left, x.Right));
        }
    }
}
