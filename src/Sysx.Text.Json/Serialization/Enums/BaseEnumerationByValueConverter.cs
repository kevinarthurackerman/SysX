namespace Sysx.Text.Json.Serialization.Enums;

/// <summary>
/// Converter for handling types extending BaseEnumeration
/// </summary>
public class BaseEnumerationByValueConverter<TEnumeration, TEnum, TValue> : JsonConverter<TEnum>
    where TEnumeration : BaseEnumeration<TEnum, TValue>
    where TEnum : BaseEnumeration<TEnum, TValue>
    where TValue : IComparable<TValue>, IEquatable<TValue>, IComparable
{
    public static readonly BaseEnumerationByValueConverter<TEnumeration, TEnum, TValue> Instance = new ();

    private BaseEnumerationByValueConverter() { }

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(TEnumeration);

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        EnsureArg.IsNotNull(typeToConvert, nameof(typeToConvert));
        EnsureArg.IsNotNull(options, nameof(options));

        var converter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));

        var value = converter.Read(ref reader, typeToConvert, options);

        return BaseEnumeration<TEnum, TValue>.ParseValue(value);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        EnsureArg.IsNotNull(writer, nameof(writer));
        EnsureArg.IsNotNull(options, nameof(options));

        var converter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));

        converter.Write(writer, value.Value, options);
    }
}
