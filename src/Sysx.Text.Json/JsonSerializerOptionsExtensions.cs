namespace Sysx.Text.Json;

public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Registers converters for sequential GUID types
    /// </summary>
    public static JsonSerializerOptions UseSequentialGuids(this JsonSerializerOptions jsonSerializerOptions)
    {
        EnsureArg.IsNotNull(jsonSerializerOptions, nameof(jsonSerializerOptions));

        jsonSerializerOptions.Converters.Add(BinaryGuidJsonConverter.Instance);
        jsonSerializerOptions.Converters.Add(SqlServerGuidJsonConverter.Instance);
        jsonSerializerOptions.Converters.Add(StringGuidJsonConverter.Instance);

        return jsonSerializerOptions;
    }
}
