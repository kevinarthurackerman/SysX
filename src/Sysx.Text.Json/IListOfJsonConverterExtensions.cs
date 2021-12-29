namespace Sysx.Text.Json;

public static class IListOfJsonConverterExtensions
{
    public static void AddSequentialGuids(this IList<JsonConverter> jsonConverters)
    {
        jsonConverters.Add(BinaryGuidJsonConverter.Instance);
        jsonConverters.Add(SqlServerGuidJsonConverter.Instance);
        jsonConverters.Add(StringGuidJsonConverter.Instance);
    }
}