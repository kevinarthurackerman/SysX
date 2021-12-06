namespace Sysx.Identity;

/// <summary>
/// This type is a thin wrapper over Guid to create semi-sequential values by overriding the comparison methods to use binary sort order.
/// Using this type is appropriate when the database field is a raw or binary type.
/// </summary>
public struct BinaryGuid : IComparable, IComparable<BinaryGuid>, IEquatable<BinaryGuid>, IFormattable
{
    private readonly Guid innerGuid;

    public BinaryGuid(Guid guid)
    {
        innerGuid = guid;
    }

    public int CompareTo(BinaryGuid value)
    {
        var innerBytes = innerGuid.ToByteArray();
        var otherBytes = value.innerGuid.ToByteArray();

        for (var i = 0; i < innerBytes.Length; i++)
        {
            var comparison = innerBytes[i].CompareTo(otherBytes[i]);
            if (comparison != 0) return comparison;
        }

        return 0;
    }

    public int CompareTo(object? value)
    {
        if (value is BinaryGuid binaryGuidValue)
            return CompareTo(binaryGuidValue);

        throw new ArgumentException("Incorrect type", nameof(value));
    }

    public bool Equals(BinaryGuid g) => innerGuid.Equals(g.innerGuid);

    public override bool Equals(object? o) => o is BinaryGuid otherBinaryGuid && innerGuid.Equals(otherBinaryGuid);
    
    public override int GetHashCode() => innerGuid.GetHashCode();

    public static BinaryGuid NewGuid() => new(Guid.NewGuid());

    public static BinaryGuid NewSequentialGuid(SequentialGuidOptions? options = default) =>
        new (SequentialGuidGenerator.Next(SequentialGuidType.Binary, options));

    public static bool operator ==(BinaryGuid a, BinaryGuid b) =>  a.innerGuid == b.innerGuid;

    public static bool operator !=(BinaryGuid a, BinaryGuid b) => a.innerGuid != b.innerGuid;

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static BinaryGuid Parse(ReadOnlySpan<char> input) => new(Guid.Parse(input));
#endif

    public static BinaryGuid Parse(string input) => new(Guid.Parse(input));

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static BinaryGuid ParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format) => new(Guid.ParseExact(input, format));
#endif

    public static BinaryGuid ParseExact(string input, string format) => new(Guid.ParseExact(input, format));

    public byte[] ToByteArray() => innerGuid.ToByteArray();

    public override string ToString() => innerGuid.ToString();

    public string ToString(string? format) => innerGuid.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => innerGuid.ToString(format, provider);

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default) =>
        innerGuid.TryFormat(destination, out charsWritten, format);
#endif

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParse(ReadOnlySpan<char> input, out BinaryGuid result)
    {
        if(Guid.TryParse(input, out var guid))
        {
            result = new BinaryGuid(guid);
            return true;
        }

        result = default;
        return false;
    }
#endif

#if NET48
    public static bool TryParse(string? input, out BinaryGuid result)
#elif NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParse([NotNullWhen(true)] string? input, out BinaryGuid result)
#endif
    {
        if (Guid.TryParse(input, out var guid))
        {
            result = new BinaryGuid(guid);
            return true;
        }

        result = default;
        return false;
    }

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format, out BinaryGuid result)
    {
        if (Guid.TryParseExact(input, format, out var guid))
        {
            result = new BinaryGuid(guid);
            return true;
        }

        result = default;
        return false;
    }
#endif

#if NET48
    public static bool TryParseExact(string? input, string? format, out BinaryGuid result)
#elif NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)] string? format, out BinaryGuid result)
#endif
    {
        if (Guid.TryParseExact(input, format, out var guid))
        {
            result = new BinaryGuid(guid);
            return true;
        }

        result = default;
        return false;
    }

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public bool TryWriteBytes(Span<byte> destination) => innerGuid.TryWriteBytes(destination);
#endif

    public static implicit operator Guid(BinaryGuid x) => x.innerGuid;

    public static implicit operator BinaryGuid(Guid x) => new(x);
}