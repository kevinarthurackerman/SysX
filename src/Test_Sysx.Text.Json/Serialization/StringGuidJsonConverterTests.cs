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

    [Fact]
    public void Should_Persist_Nullable()
    {
        var options = new JsonSerializerOptions();
        options.Converters.AddSequentialGuidJsonConverters();

        var value = (StringGuid?)StringGuid.NewGuid();

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal($"\"{value}\"", json);

        var deserializedValue = JsonSerializer.Deserialize<StringGuid?>(json, options);

        Assert.Equal(value, deserializedValue);
    }

    [Fact]
    public void Should_Persist_Null()
    {
        var options = new JsonSerializerOptions();
        options.Converters.AddSequentialGuidJsonConverters();

        var value = (StringGuid?)null;

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal("null", json);

        var deserializedValue = JsonSerializer.Deserialize<StringGuid?>(json, options);

        Assert.Equal(value, deserializedValue);
    }
}