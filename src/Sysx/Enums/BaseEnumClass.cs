namespace Sysx.Enums;

/// <summary>
/// Base class for implementing type safe class enums.
/// </summary>
[DebuggerDisplay("{Value}: {DisplayName}")]
public abstract class BaseEnumClass<TEnum, TValue>
    : IComparable<TEnum>, IEquatable<TEnum>, IComparable
    where TEnum : BaseEnumClass<TEnum, TValue>
    where TValue : IComparable<TValue>, IEquatable<TValue>, IComparable
{
    private static readonly object initLock = new { };
    private static bool isInitialized = false;
    private static IEnumerable<TEnum>? all;
    private static IDictionary<TValue, TEnum>? lookupUpValue;
    private static IDictionary<string, TEnum>? lookupUpDisplayName;

    /// <summary>
    /// Returns all the values of the enum.
    /// </summary>
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

            var enums = typeof(TEnum)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(info => typeof(TEnum).IsAssignableFrom(info.FieldType))
                .Select(info => info.GetValue(null))
                .Cast<TEnum>()
                .OrderBy(x => x.Value)
                .ToArray();

            lookupUpValue = enums.ToDictionary(x => x.Value);
            lookupUpDisplayName = enums.ToDictionary(x => x.DisplayName);

            isInitialized = true;
        }
    }

    static BaseEnumClass()
    {
        var publicConstructors = typeof(TEnum).GetConstructors(BindingFlags.Public);

        Ensure.That(!publicConstructors.Any(),
            optsFn: x => x.WithMessage($"Enum class {typeof(TEnum).Name} should not have a public constructor."));
    }

    protected BaseEnumClass(TValue value, string displayName)
    {
        EnsureArg.HasValue(value, nameof(value));
        EnsureArg.IsNotNullOrWhiteSpace(displayName, nameof(displayName));

        Value = value;
        DisplayName = displayName;
    }

    public TValue Value { get; }

    public string DisplayName { get; }

    public int CompareTo(TEnum? other) =>
        Value.CompareTo(other is default(TEnum) ? default : other.Value);

    public int CompareTo(object? other) =>
        Value.CompareTo(other is default(TEnum) ? default : other);

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
        EnsureArg.HasValue(value, nameof(value));

        Initialize();

        if(!lookupUpValue!.TryGetValue(value, out var enumValue))
        {
            throw new ArgumentException($"No enum of type {typeof(TEnum).Name} exists with value {value}");
        }

        return enumValue;
    }

    public static bool TryParseValue(TValue value, out TEnum? enumValue)
    {
        EnsureArg.HasValue(value, nameof(value));

        Initialize();
        return lookupUpValue!.TryGetValue(value, out enumValue);
    }

    public static TEnum Parse(string displayName)
    {
        EnsureArg.IsNotNullOrWhiteSpace(displayName, nameof(displayName));

        Initialize();

        if (!lookupUpDisplayName!.TryGetValue(displayName, out var enumValue))
        {
            throw new ArgumentException($"No enum of type {typeof(TEnum).Name} exists with name {displayName}");
        }

        return enumValue;
    }

    public static bool TryParse(string displayName, out TEnum? enumValue)
    {
        EnsureArg.IsNotNullOrWhiteSpace(displayName, nameof(displayName));

        Initialize();

        return lookupUpDisplayName!.TryGetValue(displayName, out enumValue);
    }

    protected virtual bool ValueEquals(TValue value)
    {
        EnsureArg.HasValue(value, nameof(value));

        return Value.Equals(value);
    }
}