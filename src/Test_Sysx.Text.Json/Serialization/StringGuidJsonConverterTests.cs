namespace Test_Sysx.Text.Json.Serialization;
using Assert = Assert;

public class StringGuidJsonConverterTests
{
    [Fact]
    public void Should_Persist()
    {
        var options = new JsonSerializerOptions();
        options.Converters.AddSequentialGuidJsonConverters();

        var value = StringGuid.NewGuid();

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal($"\"{value}\"", json);

        var deserializedValue = JsonSerializer.Deserialize<StringGuid>(json, options);

        Assert.Equal(value, deserializedValue);
    }
}