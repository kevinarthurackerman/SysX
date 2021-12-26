namespace Sysx.Text.Json.Serialization;

public class StringGuidJsonConverter : JsonConverter<StringGuid>
{
    public static readonly StringGuidJsonConverter Instance = new();

    private StringGuidJsonConverter() { }

    public override StringGuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = (JsonConverter<Guid>)options.GetConverter(typeof(Guid));

        var guid = converter.Read(ref reader, typeToConvert, options);

        return new StringGuid(guid);
    }

    public override void Write(Utf8JsonWriter writer, StringGuid value, JsonSerializerOptions options)
    {
        var converter = (JsonConverter<Guid>)options.GetConverter(typeof(Guid));

        var guid = (Guid)value;

        converter.Write(writer, guid, options);
    }
}
