namespace Sysx.Text.Json.Serialization;

public static class IListOfJsonConverterExtensions
{
    public static void AddSequentialGuidJsonConverters(this IList<JsonConverter> jsonConverters)
    {
        jsonConverters.AddBinaryGuidJsonConverter();
        jsonConverters.AddSqlServerGuidJsonConverter();
        jsonConverters.AddStringGuidJsonConverter();
    }

    public static void AddBinaryGuidJsonConverter(this IList<JsonConverter> jsonConverters) =>
        jsonConverters.Add(new BinaryGuidJsonConverter());

    public static void AddSqlServerGuidJsonConverter(this IList<JsonConverter> jsonConverters) =>
        jsonConverters.Add(new SqlServerGuidJsonConverter());

    public static void AddStringGuidJsonConverter(this IList<JsonConverter> jsonConverters) =>
        jsonConverters.Add(new StringGuidJsonConverter());
}