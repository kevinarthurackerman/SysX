namespace Sysx.Identifiers;

/// <summary>
/// This type is a thin wrapper over Guid to create semi-sequential values by overriding the comparison methods to use string sort order.
/// Using this type is appropriate when the database field is a character string or text type.
/// </summary>
public struct StringGuid : IComparable, IComparable<StringGuid>, IEquatable<StringGuid>, IFormattable
{
    public static readonly StringGuid Empty = new (Guid.Empty);

    private readonly Guid innerGuid;

    public StringGuid(Guid guid)
    {
        innerGuid = guid;
    }

    public int CompareTo(StringGuid value) => innerGuid.CompareTo(value.innerGuid);

    public int CompareTo(object? value) => innerGuid.CompareTo(value);

    public bool Equals(StringGuid g) => innerGuid.Equals(g.innerGuid);

    public override bool Equals(object? o) => innerGuid.Equals(o);
    
    public override int GetHashCode() => innerGuid.GetHashCode();

    public static StringGuid NewGuid() => new(Guid.NewGuid());

    public static StringGuid NewSequentialGuid(SequentialGuidOptions? options = default) =>
        new (SequentialGuidGenerator.Next(SequentialGuidType.String, options));

    public static bool operator ==(StringGuid a, StringGuid b) =>  a.innerGuid == b.innerGuid;

    public static bool operator !=(StringGuid a, StringGuid b) => a.innerGuid != b.innerGuid;

#if NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static StringGuid Parse(ReadOnlySpan<char> input) => new(Guid.Parse(input));
#endif

    public static StringGuid Parse(string input) => new(Guid.Parse(input));

#if NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static StringGuid ParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format) => new(Guid.ParseExact(input, format));
#endif

    public static StringGuid ParseExact(string input, string format) => new(Guid.ParseExact(input, format));

    public byte[] ToByteArray() => innerGuid.ToByteArray();

    public override string ToString() => innerGuid.ToString();

    public string ToString(string? format) => innerGuid.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => innerGuid.ToString(format, provider);

#if NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default) =>
        innerGuid.TryFormat(destination, out charsWritten, format);
#endif

#if NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParse(ReadOnlySpan<char> input, out StringGuid result)
    {
        if(Guid.TryParse(input, out var guid))
        {
            result = new StringGuid(guid);
            return true;
        }

        result = default;
        return false;
    }
#endif

#if NET48
    public static bool TryParse(string? input, out StringGuid result)
#elif NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParse([NotNullWhen(true)] string? input, out StringGuid result)
#endif
    {
        if (Guid.TryParse(input, out var guid))
        {
            result = new StringGuid(guid);
            return true;
        }

        result = default;
        return false;
    }

#if NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format, out StringGuid result)
    {
        if (Guid.TryParseExact(input, format, out var guid))
        {
            result = new StringGuid(guid);
            return true;
        }

        result = default;
        return false;
    }
#endif

#if NET48
    public static bool TryParseExact(string? input, string? format, out StringGuid result)
#elif NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)] string? format, out StringGuid result)
#endif
    {
        if (Guid.TryParseExact(input, format, out var guid))
        {
            result = new StringGuid(guid);
            return true;
        }

        result = default;
        return false;
    }

#if NET6_0 || NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public bool TryWriteBytes(Span<byte> destination) => innerGuid.TryWriteBytes(destination);
#endif

    public static implicit operator Guid(StringGuid x) => x.innerGuid;

    public static implicit operator StringGuid(Guid x) => new(x);
}