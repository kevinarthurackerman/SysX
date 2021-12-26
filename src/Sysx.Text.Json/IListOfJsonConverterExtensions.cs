namespace Sysx.Text.Json;

public static class IListOfJsonConverterExtensions
{
    public static void AddSequentialGuids(this IList<JsonConverter> jsonConverters)
    {
        jsonConverters.AddBinaryGuidJsonConverter();
        jsonConverters.AddSqlServerGuidJsonConverter();
        jsonConverters.AddStringGuidJsonConverter();
    }

    public static void AddBinaryGuidJsonConverter(this IList<JsonConverter> jsonConverters) =>
        jsonConverters.Add(BinaryGuidJsonConverter.Instance);

    public static void AddSqlServerGuidJsonConverter(this IList<JsonConverter> jsonConverters) =>
        jsonConverters.Add(SqlServerGuidJsonConverter.Instance);

    public static void AddStringGuidJsonConverter(this IList<JsonConverter> jsonConverters) =>
        jsonConverters.Add(StringGuidJsonConverter.Instance);
}