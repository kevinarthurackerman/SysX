namespace Sysx.Linq.Expressions;

/// <summary>
/// Helper to generate functions during <see langword="class"/> initialization that if valid will only <see langword="throw" /> an <see cref="Exception"/> if called.
/// </summary>
public static class ExpressionX
{
    /// <summary>
    /// Creates a nullary <see cref="Func{TResult}"/>.
    /// </summary>
    public static Func<TResult> Function<TResult>(Func<Expression> func, string? operatorName = null)
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
    /// Creates a unary <see cref="Func{TResult}"/>.
    /// </summary>
    public static Func<TValue, TResult> Function<TValue, TResult>(Func<ParameterExpression, Expression> func, string? operatorName = null)
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
    /// Creates a binary <see cref="Func{TResult}"/>.
    /// </summary>
    public static Func<TLeft, TRight, TResult> Function<TLeft, TRight, TResult>(Func<ParameterExpression, ParameterExpression, Expression> func, string? operatorName = null)
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