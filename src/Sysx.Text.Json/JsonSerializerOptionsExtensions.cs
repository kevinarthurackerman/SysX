namespace Sysx.Text.Json;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions UseSequentialGuids(this JsonSerializerOptions jsonSerializerOptions)
    {
        EnsureArg.IsNotNull(jsonSerializerOptions, nameof(jsonSerializerOptions));

        jsonSerializerOptions.Converters.Add(BinaryGuidJsonConverter.Instance);
        jsonSerializerOptions.Converters.Add(SqlServerGuidJsonConverter.Instance);
        jsonSerializerOptions.Converters.Add(StringGuidJsonConverter.Instance);

        return jsonSerializerOptions;
    }
}
