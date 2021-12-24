namespace Sysx.Text.Json.Serialization;

public class BinaryGuidJsonConverter : JsonConverter<BinaryGuid>
{
    public override BinaryGuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = (JsonConverter<Guid>)options.GetConverter(typeof(Guid));

        var guid = converter.Read(ref reader, typeToConvert, options);

        return new BinaryGuid(guid);
    }

    public override void Write(Utf8JsonWriter writer, BinaryGuid value, JsonSerializerOptions options)
    {
        var converter = (JsonConverter<Guid>)options.GetConverter(typeof(Guid));

        var guid = (Guid)value;

        converter.Write(writer, guid, options);
    }
}
