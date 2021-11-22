using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sysx.Expressions;

namespace Sysx.Enums
{
    public static class FlagsEnum
    {
        public static bool Has<TEnum>(TEnum current, TEnum flag) => FlagsEnum<TEnum>.Has(current, flag);

        public static bool HasAny<TEnum>(TEnum current, TEnum flags) => FlagsEnum<TEnum>.HasAny(current, flags);

        public static TEnum Add<TEnum>(TEnum current, TEnum flags) => FlagsEnum<TEnum>.Add(current, flags);

        public static TEnum Remove<TEnum>(TEnum current, TEnum flags) => FlagsEnum<TEnum>.Remove(current, flags);

        public static TEnum None<TEnum>() => FlagsEnum<TEnum>.None;

        public static TEnum All<TEnum>() => FlagsEnum<TEnum>.All;

        public static IEnumerable<TEnum> Expand<TEnum>(TEnum value) => FlagsEnum<TEnum>.Expand(value);

        public static TEnum Combine<TEnum>(params TEnum[] values) => FlagsEnum<TEnum>.Combine(values);
    }

    public static class FlagsEnum<TEnum>
    {
        private static readonly Func<TEnum, TEnum, bool> has =
            Lambda.Create<TEnum, TEnum, bool>((current, flag) =>
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
            Lambda.Create<TEnum, TEnum, bool>((current, flag) =>
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
            Lambda.Create<TEnum, TEnum, TEnum>((current, flags) =>
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
            Lambda.Create<TEnum, TEnum, TEnum>((current, flags) =>
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

        private static readonly IEnumerable<TEnum> values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        public static bool Has(TEnum current, TEnum flag) => has(current, flag);

        public static bool HasAny(TEnum current, TEnum flags) => hasAny(current, flags);

        public static TEnum Add(TEnum current, TEnum flags) => add(current, flags);

        public static TEnum Remove(TEnum current, TEnum flags) => remove(current, flags);

        public static TEnum None { get; } = (TEnum)Convert.ChangeType(0, typeof(TEnum).GetEnumUnderlyingType());

        public static TEnum All { get; } = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Aggregate(None, (l, r) => Add(l, r));

        public static IEnumerable<TEnum> Expand(TEnum value) => values.Where(x => Has(value, x)).ToArray();

        public static TEnum Combine(params TEnum[] values) => values.Aggregate(None, (left, right) => Add(left, right));
    }
}
