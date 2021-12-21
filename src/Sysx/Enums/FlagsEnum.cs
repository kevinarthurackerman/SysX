namespace Sysx.Enums;

/// <inheritdoc cref="FlagsEnum{TEnum}"/>
public static class FlagsEnum
{
    /// <inheritdoc cref="FlagsEnum{TEnum}.None"/>
    public static TEnum None<TEnum>() where TEnum : Enum =>
        FlagsEnum<TEnum>.None;

    /// <inheritdoc cref="FlagsEnum{TEnum}.Values"/>
    public static IEnumerable<TEnum> Values<TEnum>() where TEnum : Enum =>
        FlagsEnum<TEnum>.Values;

    /// <inheritdoc cref="FlagsEnum{TEnum}.All"/>
    public static TEnum All<TEnum>() where TEnum : Enum =>
        FlagsEnum<TEnum>.All;

    /// <inheritdoc cref="FlagsEnum{TEnum}.Expand(TEnum)"/>
    public static IEnumerable<TEnum> Expand<TEnum>(this TEnum value) where TEnum : Enum =>
        FlagsEnum<TEnum>.Expand(value);

    /// <inheritdoc cref="FlagsEnum{TEnum}.Combine(TEnum[])"/>
    public static TEnum Combine<TEnum>(params TEnum[] values) where TEnum : Enum =>
        FlagsEnum<TEnum>.Combine(values);

    /// <inheritdoc cref="FlagsEnum{TEnum}.Has(TEnum, TEnum)"/>
    public static bool Has<TEnum>(this TEnum current, TEnum flag) where TEnum : Enum =>
        FlagsEnum<TEnum>.Has(current, flag);

    /// <inheritdoc cref="FlagsEnum{TEnum}.HasAll(TEnum, TEnum)"/>
    public static bool HasAll<TEnum>(this TEnum current, params TEnum[] flags) where TEnum : Enum =>
        FlagsEnum<TEnum>.HasAll(current, flags);

    /// <inheritdoc cref="FlagsEnum{TEnum}.HasAll(TEnum, TEnum)"/>
    public static bool HasAll<TEnum>(this TEnum current, TEnum flags) where TEnum : Enum =>
        FlagsEnum<TEnum>.HasAll(current, flags);

    /// <inheritdoc cref="FlagsEnum{TEnum}.HasAny(TEnum, TEnum)"/>
    public static bool HasAny<TEnum>(this TEnum current, params TEnum[] flags) where TEnum : Enum =>
        FlagsEnum<TEnum>.HasAny(current, flags);

    /// <inheritdoc cref="FlagsEnum{TEnum}.HasAny(TEnum, TEnum)"/>
    public static bool HasAny<TEnum>(this TEnum current, TEnum flags) where TEnum : Enum =>
        FlagsEnum<TEnum>.HasAny(current, flags);

    /// <inheritdoc cref="FlagsEnum{TEnum}.Add(TEnum, TEnum)"/>
    public static TEnum Add<TEnum>(this TEnum current, params TEnum[] flags) where TEnum : Enum =>
        FlagsEnum<TEnum>.Add(current, flags);

    /// <inheritdoc cref="FlagsEnum{TEnum}.Add(TEnum, TEnum)"/>
    public static TEnum Add<TEnum>(this TEnum current, TEnum flags) where TEnum : Enum =>
        FlagsEnum<TEnum>.Add(current, flags);

    /// <inheritdoc cref="FlagsEnum{TEnum}.Remove(TEnum, TEnum)"/>
    public static TEnum Remove<TEnum>(this TEnum current, params TEnum[] flags) where TEnum : Enum =>
        FlagsEnum<TEnum>.Remove(current, flags);

    /// <inheritdoc cref="FlagsEnum{TEnum}.Remove(TEnum, TEnum)"/>
    public static TEnum Remove<TEnum>(this TEnum current, TEnum flags) where TEnum : Enum =>
        FlagsEnum<TEnum>.Remove(current, flags);
}

/// <summary>
/// Provides operations for manipulating flags enums
/// </summary>
public static class FlagsEnum<TEnum> where TEnum : Enum
{
    private static readonly Func<TEnum, TEnum, bool> hasAll =
        ExpressionX.Function<TEnum, TEnum, bool>((current, flag) =>
        {
            // current & flag == flag
            var realType = typeof(TEnum).GetEnumUnderlyingType();
            var currentConv = Expression.Convert(current, realType);
            var flagConv = Expression.Convert(flag, realType);
            var and = Expression.And(currentConv, flagConv);
            var equal = Expression.Equal(and, flagConv);
            return equal;
        }, nameof(FlagsEnum<TEnum>.HasAll));

    private static readonly Func<TEnum, TEnum, bool> hasAny =
        ExpressionX.Function<TEnum, TEnum, bool>((current, flag) =>
        {
            // current & flag != 0
            var realType = typeof(TEnum).GetEnumUnderlyingType();
            var currentConv = Expression.Convert(current, realType);
            var flagConv = Expression.Convert(flag, realType);
            var and = Expression.And(currentConv, flagConv);
            var zero = Expression.Convert(Expression.Constant(0), realType);
            var notEqual = Expression.NotEqual(and, zero);
            return notEqual;
        }, nameof(FlagsEnum<TEnum>.HasAny));

    private static readonly Func<TEnum, TEnum, TEnum> add =
        ExpressionX.Function<TEnum, TEnum, TEnum>((current, flags) =>
        {
            // current | flag
            var realType = typeof(TEnum).GetEnumUnderlyingType();
            var currentConv = Expression.Convert(current, realType);
            var flagsConv = Expression.Convert(flags, realType);
            var or = Expression.Or(currentConv, flagsConv);
            var convertResult = Expression.Convert(or, typeof(TEnum));
            return convertResult;
        }, nameof(FlagsEnum<TEnum>.Add));

    private static readonly Func<TEnum, TEnum, TEnum> remove =
        ExpressionX.Function<TEnum, TEnum, TEnum>((current, flags) =>
        {
            // current & ~flags
            var realType = typeof(TEnum).GetEnumUnderlyingType();
            var currentConv = Expression.Convert(current, realType);
            var flagsConv = Expression.Convert(flags, realType);
            var onesComplement = Expression.OnesComplement(flagsConv);
            var and = Expression.And(currentConv, onesComplement);
            var convertResult = Expression.Convert(and, typeof(TEnum));
            return convertResult;
        }, nameof(FlagsEnum<TEnum>.Remove));

    /// <summary>
    /// A value representing no flags set.
    /// </summary>
    public static TEnum None { get; } = (TEnum)Convert.ChangeType(0, typeof(TEnum).GetEnumUnderlyingType());

    /// <summary>
    /// A list of all the defined values of the enum.
    /// </summary>
    public static IEnumerable<TEnum> Values { get; } = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

    /// <summary>
    /// A value representing all values of the flag being set.
    /// </summary>
    public static TEnum All { get; } = CombineSkipCheck(Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray());

    /// <summary>
    /// Expands the flag value to a list of defined flags.
    /// </summary>
    public static IEnumerable<TEnum> Expand(TEnum value)
    {
        EnsureArg.HasValue(value, nameof(value));

        return Values.Where(x => HasAll(value, x)).ToArray();
    }

    /// <summary>
    /// Combines the flag values into a single flag.
    /// </summary>
    public static TEnum Combine(params TEnum[] values)
    {
        EnsureArg.IsNotNull(values, nameof(values));

        return CombineSkipCheck(values);
    }

    /// <summary>
    /// Checks if the current flag value contains the flag.
    /// </summary>
    public static bool Has(TEnum current, TEnum flag)
    {
        EnsureArg.HasValue(current, nameof(current));
        EnsureArg.HasValue(flag, nameof(flag));

        var flagValues = Expand(flag).ToArray();

        EnsureArg.SizeIs(flagValues, 1, nameof(flag), x => x.WithMessage($"{nameof(flag)} must contain one and only one flag value."));

        return hasAny(current, flag);
    }

    /// <inheritdoc cref="FlagsEnum{TEnum}.HasAll(TEnum, TEnum)"/>
    public static bool HasAll(TEnum current, params TEnum[] flags) =>
        HasAll(current, CombineSkipCheck(flags));

    /// <summary>
    /// Checks if the current flag value contains all of the flags.
    /// </summary>
    public static bool HasAll(TEnum current, TEnum flags)
    {
        EnsureArg.HasValue(current, nameof(current));
        EnsureArg.HasValue(flags, nameof(flags));

        return hasAll(current, flags);
    }

    /// <inheritdoc cref="FlagsEnum{TEnum}.HasAny(TEnum, TEnum)"/>
    public static bool HasAny(TEnum current, params TEnum[] flags) =>
        HasAny(current, CombineSkipCheck(flags));

    /// <summary>
    /// Checks if the current flag value contains any of the flags.
    /// </summary>
    public static bool HasAny(TEnum current, TEnum flags) 
    {
        EnsureArg.HasValue(current, nameof(current));
        EnsureArg.HasValue(flags, nameof(flags));

        return hasAny(current, flags);
    }

    /// <inheritdoc cref="FlagsEnum{TEnum}.Add(TEnum, TEnum)"/>
    public static TEnum Add(TEnum current, params TEnum[] flags) =>
        Add(current, CombineSkipCheck(flags));

    /// <summary>
    /// Adds the flags value to the current flag.
    /// </summary>
    public static TEnum Add(TEnum current, TEnum flags)
    {
        EnsureArg.HasValue(current, nameof(current));
        EnsureArg.HasValue(flags, nameof(flags));

        return add(current, flags);
    }

    /// <inheritdoc cref="FlagsEnum{TEnum}.Remove(TEnum, TEnum)"/>
    public static TEnum Remove(TEnum current, params TEnum[] flags) =>
        Remove(current, CombineSkipCheck(flags));

    /// <summary>
    /// Removes the flags value from the current flag.
    /// </summary>
    public static TEnum Remove(TEnum current, TEnum flags)
    {
        EnsureArg.HasValue(current, nameof(current));
        EnsureArg.HasValue(flags, nameof(flags));

        return remove(current, flags);
    }

    private static TEnum CombineSkipCheck(params TEnum[] values)
    {
        var combinedFlag = None;

        for(var i = 0; i < values.Length; i++)
            combinedFlag = add(combinedFlag, values[i]);

        return combinedFlag;
    }
}