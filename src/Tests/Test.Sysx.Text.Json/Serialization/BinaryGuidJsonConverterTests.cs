namespace Test.Sysx.Text.Json.Serialization;

public class BinaryGuidJsonConverterTests
{
    [Fact]
    public void Should_Persist()
    {
        var options = new JsonSerializerOptions();
        options.Converters.AddSequentialGuidJsonConverters();

        var value = BinaryGuid.NewGuid();

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal($"\"{value}\"", json);

        var deserializedValue = JsonSerializer.Deserialize<BinaryGuid>(json, options);

        Assert.Equal(value, deserializedValue);
    }
}