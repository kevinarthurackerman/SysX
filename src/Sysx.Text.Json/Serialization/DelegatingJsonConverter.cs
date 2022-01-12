namespace Sysx.Text.Json.Serialization.Identifiers;

/// <summary>
/// Converter to delegate to another converter.
/// </summary>
public class DelegatingJsonConverter<TValue, TSerializableValue> : JsonConverter<TValue>
{
    private readonly Func<TValue?, TSerializableValue?> convertToSerializable;
    private readonly Func<TSerializableValue?, TValue?> convertFromSerializable;

    public DelegatingJsonConverter()
    {
        if(!Cast.CanCast<TValue, TSerializableValue>() || !Cast.CanCast<TSerializableValue, TValue>())
        {
            throw new NotSupportedException($"{typeof(TValue)} and {typeof(TSerializableValue)} must be castable to each other, or conversion functions must be provided in the constructor.");
        }

        convertToSerializable = x => Cast.Value<TValue, TSerializableValue>(x);
        convertFromSerializable = x => Cast.Value<TSerializableValue, TValue>(x);
    }

    public DelegatingJsonConverter(Func<TValue?, TSerializableValue?> convertToSerializable, Func<TSerializableValue?, TValue?> convertFromSerializable)
    {
        EnsureArg.IsNotNull(convertToSerializable, nameof(convertToSerializable));
        EnsureArg.IsNotNull(convertFromSerializable, nameof(convertFromSerializable));

        this.convertToSerializable = convertToSerializable;
        this.convertFromSerializable = convertFromSerializable;
    }

    public override TValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        EnsureArg.IsNotNull(typeToConvert, nameof(typeToConvert));
        EnsureArg.IsNotNull(options, nameof(options));

        var converter = (JsonConverter<TSerializableValue>)options.GetConverter(typeof(TSerializableValue));

        var serializableValue = converter.Read(ref reader, typeToConvert, options);

        return convertFromSerializable(serializableValue);
    }

    public override void Write(Utf8JsonWriter writer, TValue value, JsonSerializerOptions options)
    {
        EnsureArg.IsNotNull(writer, nameof(writer));
        EnsureArg.IsNotNull(options, nameof(options));

        var converter = (JsonConverter<TSerializableValue>)options.GetConverter(typeof(TSerializableValue));

        var serializableValue = convertToSerializable(value);

#pragma warning disable CS8604 // Possible null reference argument.
        converter.Write(writer, serializableValue, options);
#pragma warning restore CS8604 // Possible null reference argument.
    }
}
