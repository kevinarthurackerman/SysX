using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Sysx.Enums
{
    [DebuggerDisplay("{Value}: {DisplayName}")]
    public abstract class BaseEnumClass<TEnum, TValue> : IComparable<TEnum>, IEquatable<TEnum>
        where TEnum : BaseEnumClass<TEnum, TValue>
        where TValue : IComparable
    {
        private static readonly object initLock = new { };
        private static bool isInitialized = false;
        private static IEnumerable<TEnum>? all;
        private static IDictionary<TValue, TEnum>? lookupUpValue;
        private static IDictionary<string, TEnum>? lookupUpDisplayName;

        public static IEnumerable<TEnum> All
        {   
            get
            {
                Initialize();
                return all!;
            }
        }

        private static void Initialize()
        {
            if (isInitialized) return;

            lock (initLock)
            {
                if (isInitialized) return;

                all = typeof(TEnum)
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .Where(info => typeof(TEnum).IsAssignableFrom(info.FieldType))
                    .Select(info => info.GetValue(null))
                    .Concat(
                        typeof(TEnum)
                            .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                            .Where(info => typeof(TEnum).IsAssignableFrom(info.PropertyType))
                            .Select(info => info.GetValue(null))
                            .ToArray()
                    )
                    .Cast<TEnum>()
                    .OrderBy(x => x.Value)
                    .ToArray();

                lookupUpValue = all.ToDictionary(x => x.Value);
                lookupUpDisplayName = all.ToDictionary(x => x.DisplayName);

                isInitialized = true;
            }
        }

        static BaseEnumClass()
        {
            if (typeof(TEnum).GetConstructors(BindingFlags.Public).Any())
                throw new InvalidOperationException($"Enum class {typeof(TEnum).Name} should not have a public constructor.");
        }

        protected BaseEnumClass(TValue value, string displayName)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            Value = value;
            DisplayName = displayName;
        }

        public TValue Value { get; }

        public string DisplayName { get; }

        public int CompareTo(TEnum? other) =>
            Value.CompareTo(other is default(TEnum) ? default : other.Value);

        public override sealed string ToString() => DisplayName;

        public override bool Equals(object? obj) => Equals(obj as TEnum);

        public bool Equals(TEnum? other) => !(other is default(TEnum)) && ValueEquals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(BaseEnumClass<TEnum, TValue> left, BaseEnumClass<TEnum, TValue> right) =>
            Equals(left, right);

        public static bool operator !=(BaseEnumClass<TEnum, TValue> left, BaseEnumClass<TEnum, TValue> right) =>
            !Equals(left, right);

        public static TEnum ParseValue(TValue value)
        {
            Initialize();

            if(!TryParseValue(value, out var result))
            {
                throw new ArgumentException($"No enum of type {typeof(TEnum).Name} exists with value {value}");
            }

            return result!;
        }

        public static bool TryParseValue(TValue value, out TEnum? enumValue)
        {
            Initialize();
            return lookupUpValue!.TryGetValue(value, out enumValue);
        }

        public static TEnum Parse(string displayName)
        {
            Initialize();

            if (!TryParse(displayName, out var result))
            {
                throw new ArgumentException($"No enum of type {typeof(TEnum).Name} exists with name {displayName}");
            }

            return result!;
        }

        public static bool TryParse(string displayName, out TEnum? enumValue)
        {
            Initialize();
            return lookupUpDisplayName!.TryGetValue(displayName, out enumValue);
        }

        protected virtual bool ValueEquals(TValue value) => Value.Equals(value);
    }
}
