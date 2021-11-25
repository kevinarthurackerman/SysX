using EnsureThat;
using System;
using System.Linq.Expressions;

namespace Sysx.Expressions
{
    using Expression = System.Linq.Expressions.Expression;

    /// <summary>
    /// Helper to generate functions during class initialization that if valid will only throw an exception if called.
    /// </summary>
    public static class Lambda
    {
        /// <summary>
        /// Creates a nullary function
        /// </summary>
        public static Func<TResult> Create<TResult>(Func<Expression> func, string? operatorName = null)
        {
            EnsureArg.IsNotNull(func, nameof(func));

            try
            {
                var expression = func();

                return Expression.Lambda<Func<TResult>>(expression).Compile();
            }
            catch (Exception)
            {
                return () => throw new InvalidOperationException($"No operator exists for {operatorName ?? string.Empty}() => {typeof(TResult).Name}.");
            }
        }

        /// <summary>
        /// Creates a unary function
        /// </summary>
        public static Func<TValue, TResult> Create<TValue, TResult>(Func<ParameterExpression, Expression> func, string? operatorName = null)
        {
            EnsureArg.IsNotNull(func, nameof(func));

            try
            {
                var valueParam = Expression.Parameter(typeof(TValue), "value");

                var expression = func(valueParam);

                if (typeof(TValue) != typeof(TResult))
                {
                    try
                    {
                        expression = Expression.Convert(expression, typeof(TResult));
                    }
                    catch (Exception)
                    {
                        return (value) => throw new InvalidCastException($"Operator {operatorName ?? string.Empty}({typeof(TValue).Name}) could not cast to result type {typeof(TResult).Name}.");
                    }
                }

                return Expression.Lambda<Func<TValue, TResult>>(expression, valueParam).Compile();
            }
            catch (Exception)
            {
                return (value) => throw new InvalidOperationException($"No operator exists for {operatorName ?? string.Empty}({typeof(TValue).Name}) => {typeof(TResult).Name}.");
            }
        }

        /// <summary>
        /// Creates a binary function
        /// </summary>
        public static Func<TLeft, TRight, TResult> Create<TLeft, TRight, TResult>(Func<ParameterExpression, ParameterExpression, Expression> func, string? operatorName = null)
        {
            EnsureArg.IsNotNull(func, nameof(func));

            try
            {
                var leftParam = Expression.Parameter(typeof(TLeft), "left");
                var rightParam = Expression.Parameter(typeof(TRight), "right");

                var expression = func(leftParam, rightParam);

                if (typeof(TLeft) != typeof(TResult) || typeof(TRight) != typeof(TResult))
                {
                    try
                    {
                        expression = Expression.Convert(expression, typeof(TResult));
                    }
                    catch (Exception)
                    {
                        return (left, right) => throw new InvalidCastException($"Operator {operatorName ?? string.Empty}({typeof(TLeft).Name}, {typeof(TRight).Name}) could not cast to result type {typeof(TResult).Name}.");
                    }
                }

                return Expression.Lambda<Func<TLeft, TRight, TResult>>(expression, leftParam, rightParam).Compile();
            }
            catch (Exception)
            {
                return (left, right) => throw new InvalidOperationException($"No operator exists for {operatorName ?? string.Empty}({typeof(TLeft).Name}, {typeof(TRight).Name}) => {typeof(TResult).Name}.");
            }
        }
    }
}
