using System;
using System.Linq.Expressions;

namespace Sysx.Expressions
{
    using Expression = System.Linq.Expressions.Expression;

    public static class Lambda
    {
        public static Func<TResult> Create<TResult>(Func<Expression> func, string operatorName)
        {
            try
            {
                var expression = func();

                return Expression.Lambda<Func<TResult>>(expression).Compile();
            }
            catch (Exception)
            {
                return () => throw new InvalidOperationException($"No operator exists for {operatorName}() => {typeof(TResult).Name}.");
            }
        }

        public static Func<TValue, TResult> Create<TValue, TResult>(Func<ParameterExpression, Expression> func, string operatorName)
        {
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
                        return (value) => throw new InvalidCastException($"Operator {operatorName}({typeof(TValue).Name}) could not cast to result type {typeof(TResult).Name}.");
                    }
                }

                return Expression.Lambda<Func<TValue, TResult>>(expression, valueParam).Compile();
            }
            catch (Exception)
            {
                return (value) => throw new InvalidOperationException($"No operator exists for {operatorName}({typeof(TValue).Name}) => {typeof(TResult).Name}.");
            }
        }

        public static Func<TLeft, TRight, TResult> Create<TLeft, TRight, TResult>(Func<ParameterExpression, ParameterExpression, Expression> func, string operatorName)
        {
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
                        return (left, right) => throw new InvalidCastException($"Operator {operatorName}({typeof(TLeft).Name}, {typeof(TRight).Name}) could not cast to result type {typeof(TResult).Name}.");
                    }
                }

                return Expression.Lambda<Func<TLeft, TRight, TResult>>(expression, leftParam, rightParam).Compile();
            }
            catch (Exception)
            {
                return (left, right) => throw new InvalidOperationException($"No operator exists for {operatorName}({typeof(TLeft).Name}, {typeof(TRight).Name}) => {typeof(TResult).Name}.");
            }
        }
    }
}
