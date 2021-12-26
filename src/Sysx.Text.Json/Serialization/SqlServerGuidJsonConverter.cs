﻿namespace Sysx.Text.Json.Serialization;

public class SqlServerGuidJsonConverter : JsonConverter<SqlServerGuid>
{
    public static readonly SqlServerGuidJsonConverter Instance = new();

    private SqlServerGuidJsonConverter() { }

    public override SqlServerGuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = (JsonConverter<Guid>)options.GetConverter(typeof(Guid));

        var guid = converter.Read(ref reader, typeToConvert, options);

        return new SqlServerGuid(guid);
    }

    public override void Write(Utf8JsonWriter writer, SqlServerGuid value, JsonSerializerOptions options)
    {
        var converter = (JsonConverter<Guid>)options.GetConverter(typeof(Guid));

        var guid = (Guid)value;

        converter.Write(writer, guid, options);
    }
}
