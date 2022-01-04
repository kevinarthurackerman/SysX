namespace Sysx.Text.Json.Serialization.Enums;

/// <summary>
/// Converter for handling types extending BaseEnumeration
/// </summary>
public class BaseEnumerationByDisplayNameConverter<TEnumeration, TEnum, TValue> : JsonConverter<TEnum>
    where TEnumeration : BaseEnumeration<TEnum, TValue>
    where TEnum : BaseEnumeration<TEnum, TValue>
    where TValue : IComparable<TValue>, IEquatable<TValue>, IComparable
{
    public static readonly BaseEnumerationByDisplayNameConverter<TEnumeration, TEnum, TValue> Instance = new ();

    private BaseEnumerationByDisplayNameConverter() { }

    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(TEnumeration);

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        EnsureArg.IsNotNull(typeToConvert, nameof(typeToConvert));
        EnsureArg.IsNotNull(options, nameof(options));

        var converter = (JsonConverter<string>)options.GetConverter(typeof(string));

        var value = converter.Read(ref reader, typeToConvert, options);

        return BaseEnumeration<TEnum, TValue>.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        EnsureArg.IsNotNull(writer, nameof(writer));
        EnsureArg.IsNotNull(options, nameof(options));

        var converter = (JsonConverter<string>)options.GetConverter(typeof(string));

        converter.Write(writer, value.DisplayName, options);
    }
}
