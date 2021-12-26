namespace Test_Sysx.Text.Json.Serialization;
using Assert = Assert;

public class BinaryGuidJsonConverterTests
{
    [Fact]
    public void Should_Persist()
    {
        var options = new JsonSerializerOptions();
        options.Converters.AddSequentialGuids();

        var value = BinaryGuid.NewGuid();

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal($"\"{value}\"", json);

        var deserializedValue = JsonSerializer.Deserialize<BinaryGuid>(json, options);

        Assert.Equal(value, deserializedValue);
    }

    [Fact]
    public void Should_Persist_Nullable()
    {
        var options = new JsonSerializerOptions();
        options.Converters.AddSequentialGuids();

        var value = (BinaryGuid?)BinaryGuid.NewGuid();

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal($"\"{value}\"", json);

        var deserializedValue = JsonSerializer.Deserialize<BinaryGuid?>(json, options);

        Assert.Equal(value, deserializedValue);
    }

    [Fact]
    public void Should_Persist_Null()
    {
        var options = new JsonSerializerOptions();
        options.Converters.AddSequentialGuids();

        var value = (BinaryGuid?)null;

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal("null", json);

        var deserializedValue = JsonSerializer.Deserialize<BinaryGuid?>(json, options);

        Assert.Equal(value, deserializedValue);
    }
}