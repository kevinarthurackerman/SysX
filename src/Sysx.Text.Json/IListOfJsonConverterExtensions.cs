namespace Sysx.Text.Json;

public static class IListOfJsonConverterExtensions
{
    public static void AddSequentialGuids(this IList<JsonConverter> jsonConverters)
    {
        jsonConverters.AddBinaryGuids();
        jsonConverters.AddSqlServerGuids();
        jsonConverters.AddStringGuids();
    }

    public static void AddBinaryGuids(this IList<JsonConverter> jsonConverters) =>
        jsonConverters.Add(BinaryGuidJsonConverter.Instance);

    public static void AddSqlServerGuids(this IList<JsonConverter> jsonConverters) =>
        jsonConverters.Add(SqlServerGuidJsonConverter.Instance);

    public static void AddStringGuids(this IList<JsonConverter> jsonConverters) =>
        jsonConverters.Add(StringGuidJsonConverter.Instance);
}