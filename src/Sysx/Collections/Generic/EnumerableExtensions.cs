﻿using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sysx.Collections.Generic
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs a cross apply, joining all the elements of the enumerable to all the elements of the enumerable.
        /// </summary>
        public static IEnumerable<(T Left, T Right)> CrossApply<T>(this IEnumerable<T> enumerable) =>
            CrossApply(enumerable, enumerable, (left, right) => (Left: left, Right: right));

        /// <summary>
        /// Performs a cross apply, joining all the elements of the left enumerable to all the elements of the right enumerable.
        /// </summary>
        public static IEnumerable<(T Left, T Right)> CrossApply<T>(this IEnumerable<T> left, IEnumerable<T> right) =>
            CrossApply(left, right, (left, right) => (Left: left, Right: right));

        /// <summary>
        /// Performs a cross apply, joining all the elements of the left enumerable to all the elements of the right enumerable.
        /// </summary>
        public static IEnumerable<(TLeft Left, TRight Right)> CrossApply<TLeft, TRight>(this IEnumerable<TLeft> left, IEnumerable<TRight> right) =>
            CrossApply(left, right, (left, right) => (Left: left, Right: right));

        /// <summary>
        /// Performs a cross apply, joining all the elements of the left enumerable to all the elements of the right enumerable.
        /// </summary>
        public static IEnumerable<TResult> CrossApply<TLeft, TRight, TResult>(this IEnumerable<TLeft> left, IEnumerable<TRight> right, Func<TLeft,TRight,TResult> resultSelector)
        {
            EnsureArg.IsNotNull(left, nameof(left));
            EnsureArg.IsNotNull(right, nameof(right));
            EnsureArg.IsNotNull(resultSelector, nameof(resultSelector));

            return left.Join(right, x => true, x => true, (left, right) => (Left: left, Right: right))
                .Select(x => resultSelector(x.Left, x.Right));
        }

        /// <summary>
        /// Memoizes the enumerable, ensuring that it will only be evaluated up to one time, even if it is called more than once.
        /// </summary>
        public static IEnumerable<T>? Memoize<T>(this IEnumerable<T>? enumerable) => MemoizedEnumerable<T>.Create(enumerable);
    }
}