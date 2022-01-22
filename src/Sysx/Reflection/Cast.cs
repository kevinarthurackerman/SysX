namespace Sysx.Reflection;

/// <summary>
/// Used to cast a value from one <see cref="Type"/> to another.
/// Supports casting to the same <see cref="Type"/>, from an inherited <see cref="Type"/>, to an implemented <see langword="interface"/>, <see cref="IConvertible"/> casts, <see langword="explicit"/>y, and <see langword="implicit"/>y.
/// </summary>
public static class Cast<TFrom, TTo>
{
    private static readonly bool canCast;
    private static readonly Func<TFrom?, TTo?> castValue;

    static Cast()
    {
        var fromType = typeof(TFrom);
        var toType = typeof(TTo);

        if (fromType == toType
            || toType.IsAssignableFrom(fromType)
            || TypeDescriptor.GetConverter(fromType).CanConvertTo(toType))
        {
            canCast = true;
        }
        else
        {
            UnaryExpression bodyFunction(Expression body) => Expression.Convert(body, toType);
            ParameterExpression from = Expression.Parameter(fromType, "from");
            try
            {
                Expression.Lambda(bodyFunction(from), from).Compile();
                canCast = true;
            }
            catch (InvalidOperationException)
            {
                canCast = false;
            }
        }

        if (canCast)
        {
            var parameter = Expression.Parameter(typeof(TFrom), "from");
            var convert = Expression.Convert(parameter, typeof(TTo));
            var expressionBody = Expression.Block(typeof(TTo), convert);
            castValue = Expression.Lambda<Func<TFrom?, TTo?>>(expressionBody, parameter).Compile();
        }
        else
        {
            castValue = _ => throw new InvalidCastException($"{fromType} cannot be casted to {toType}");
        }
    }

    /// <summary>
    /// Checks if a cast can be made.
    /// </summary>
    public static bool CanCast() => canCast;

    /// <summary>
    /// Casts from one <see cref="Type"/> to another.
    /// Supports casting to the same <see cref="Type"/>, from an inherited <see cref="Type"/>, to an implemented <see langword="interface"/>, <see cref="IConvertible"/> casts, <see langword="explicit"/>y, and <see langword="implicit"/>y.
    /// </summary>
    public static TTo? Value(TFrom? from) => castValue(from);
}

/// <inheritdoc cref="Cast{TFrom, TTo}" />
public static class Cast<TFrom>
{
    private static readonly ConcurrentDictionary<Type, bool> canCastCache = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> castValueCache = new();

    /// <inheritdoc cref="Cast{TFrom, TTo}.CanCast" />
    public static bool CanCast<TTo>() => CanCast(typeof(TTo));

    /// <inheritdoc cref="Cast{TFrom, TTo}.CanCast" />
    public static bool CanCast(Type to) => canCastCache.GetOrAdd(to, toType =>
        (bool)typeof(Cast<,>).MakeGenericType(typeof(TFrom), toType).GetMethod(nameof(CanCast))!.Invoke(null, null)!);

    /// <inheritdoc cref="Cast{TFrom, TTo}.Value" />
    public static TTo? Value<TTo>(TFrom? value) => (TTo?)Value(typeof(TTo), value);

    /// <inheritdoc cref="Cast{TFrom, TTo}.Value" />
    public static object? Value(Type to, object? value) => castValueCache.GetOrAdd(to, toType =>
        typeof(Cast<,>).MakeGenericType(typeof(TFrom), toType).GetMethod(nameof(Value))!).Invoke(null, new[] { value });
}

/// <inheritdoc cref="Cast{TFrom, TTo}" />
public static class Cast
{
    private static readonly ConcurrentDictionary<CastCacheKey, bool> canCastCache = new();
    private static readonly ConcurrentDictionary<CastCacheKey, MethodInfo> castValueCache = new();

    /// <inheritdoc cref="Cast{TFrom, TTo}.CanCast" />
    public static bool CanCast<TFrom, TTo>() => CanCast(typeof(TFrom), typeof(TTo));

    /// <inheritdoc cref="Cast{TFrom, TTo}.CanCast" />
    public static bool CanCast(Type from, Type to) => canCastCache.GetOrAdd(new CastCacheKey(from, to), x =>
        (bool)typeof(Cast<,>).MakeGenericType(x.From, x.To).GetMethod(nameof(CanCast))!.Invoke(null, null)!);

    /// <inheritdoc cref="Cast{TFrom, TTo}.Value" />
    public static TTo? Value<TFrom, TTo>(TFrom? value) => (TTo?)Value(typeof(TFrom), typeof(TTo), value);

    /// <inheritdoc cref="Cast{TFrom, TTo}.Value" />
    public static object? Value(Type from, Type to, object? value) => castValueCache.GetOrAdd(new CastCacheKey(from, to), x =>
        typeof(Cast<,>).MakeGenericType(x.From, x.To).GetMethod(nameof(Value))!).Invoke(null, new[] { value });

    private readonly record struct CastCacheKey(Type From, Type To);
}