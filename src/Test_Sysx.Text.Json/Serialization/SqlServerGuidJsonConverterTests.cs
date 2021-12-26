namespace Test_Sysx.Text.Json.Serialization;
using Assert = Assert;

public class SqlServerGuidJsonConverterTests
{
    [Fact]
    public void Should_Persist()
    {
        var options = new JsonSerializerOptions();
        options.Converters.AddSequentialGuidJsonConverters();

        var value = SqlServerGuid.NewGuid();

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal($"\"{value}\"", json);

        var deserializedValue = JsonSerializer.Deserialize<SqlServerGuid>(json, options);

        Assert.Equal(value, deserializedValue);
    }
}