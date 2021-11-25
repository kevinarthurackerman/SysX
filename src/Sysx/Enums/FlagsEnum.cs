using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sysx.Linq.Expressions;

namespace Sysx.Enums
{
    /// <inheritdoc cref="FlagsEnum{TEnum}"/>
    public static class FlagsEnum
    {
        /// <inheritdoc cref="FlagsEnum{TEnum}.None"/>
        public static TEnum None<TEnum>() => FlagsEnum<TEnum>.None;

        /// <inheritdoc cref="FlagsEnum{TEnum}.Values"/>
        public static IEnumerable<TEnum> Values<TEnum>() => FlagsEnum<TEnum>.Values;

        /// <inheritdoc cref="FlagsEnum{TEnum}.All"/>
        public static TEnum All<TEnum>() => FlagsEnum<TEnum>.All;

        /// <inheritdoc cref="FlagsEnum{TEnum}.Expand"/>
        public static IEnumerable<TEnum> Expand<TEnum>(TEnum value) => FlagsEnum<TEnum>.Expand(value);

        /// <inheritdoc cref="FlagsEnum{TEnum}.Combine"/>
        public static TEnum Combine<TEnum>(params TEnum[] values) => FlagsEnum<TEnum>.Combine(values);

        /// <inheritdoc cref="FlagsEnum{TEnum}.Has"/>
        public static bool Has<TEnum>(TEnum current, TEnum flag) => FlagsEnum<TEnum>.Has(current, flag);

        /// <inheritdoc cref="FlagsEnum{TEnum}.HasAny"/>
        public static bool HasAny<TEnum>(TEnum current, TEnum flags) => FlagsEnum<TEnum>.HasAny(current, flags);

        /// <inheritdoc cref="FlagsEnum{TEnum}.Add"/>
        public static TEnum Add<TEnum>(TEnum current, TEnum flags) => FlagsEnum<TEnum>.Add(current, flags);

        /// <inheritdoc cref="FlagsEnum{TEnum}.Remove"/>
        public static TEnum Remove<TEnum>(TEnum current, TEnum flags) => FlagsEnum<TEnum>.Remove(current, flags);
    }

    /// <summary>
    /// Provides operations for manipulating flags enums
    /// </summary>
    public static class FlagsEnum<TEnum>
    {
        private static readonly Func<TEnum, TEnum, bool> has =
            ExpressionX.Function<TEnum, TEnum, bool>((current, flag) =>
            {
                // current & flag == flag
                var realType = typeof(TEnum).GetEnumUnderlyingType();
                var currentConv = Expression.Convert(current, realType);
                var flagConv = Expression.Convert(flag, realType);
                var and = Expression.And(currentConv, flagConv);
                var equal = Expression.Equal(and, flagConv);
                return equal;
            }, nameof(FlagsEnum<TEnum>.Has));

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
        public static TEnum All { get; } = Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Aggregate(None, (l, r) => Add(l, r));

        /// <summary>
        /// Expands the flag value to a list of defined flags.
        /// </summary>
        public static IEnumerable<TEnum> Expand(TEnum value)
        {
            EnsureArg.HasValue(value, nameof(value));

            return Values.Where(x => Has(value, x)).ToArray();
        }

        /// <summary>
        /// Combines the flag values into a single flag.
        /// </summary>
        public static TEnum Combine(params TEnum[] values)
        {
            EnsureArg.IsNotNull(values, nameof(values));

            return values.Aggregate(None, (left, right) => Add(left, right));
        }

        /// <summary>
        /// Checks if the current flag value contains a specific flag.
        /// </summary>
        public static bool Has(TEnum current, TEnum flag)
        {
            EnsureArg.HasValue(current, nameof(current));
            EnsureArg.HasValue(flag, nameof(flag));

            return has(current, flag);
        }

        /// <summary>
        /// Checks if the current flag value contains any of the flags.
        /// </summary>
        public static bool HasAny(TEnum current, TEnum flags) 
        {
            EnsureArg.HasValue(current, nameof(current));
            EnsureArg.HasValue(flags, nameof(flags));

            return hasAny(current, flags);
        }

        /// <summary>
        /// Adds the flags value to the current flag.
        /// </summary>
        public static TEnum Add(TEnum current, TEnum flags)
        {
            EnsureArg.HasValue(current, nameof(current));
            EnsureArg.HasValue(flags, nameof(flags));

            return add(current, flags);
        }

        /// <summary>
        /// Removes the flags value from the current flag.
        /// </summary>
        public static TEnum Remove(TEnum current, TEnum flags)
        {
            EnsureArg.HasValue(current, nameof(current));
            EnsureArg.HasValue(flags, nameof(flags));

            return remove(current, flags);
        }
     }
}
