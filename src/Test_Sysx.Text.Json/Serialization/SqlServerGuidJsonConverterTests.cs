namespace Test_Sysx.Text.Json.Serialization;
using Assert = Assert;

public class SqlServerGuidJsonConverterTests
{
    [Fact]
    public void Should_Persist()
    {
        var options = new JsonSerializerOptions().UseSequentialGuids();

        var value = SqlServerGuid.NewGuid();

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal($"\"{value}\"", json);

        var deserializedValue = JsonSerializer.Deserialize<SqlServerGuid>(json, options);

        Assert.Equal(value, deserializedValue);
    }

    [Fact]
    public void Should_Persist_Nullable()
    {
        var options = new JsonSerializerOptions().UseSequentialGuids();

        var value = (SqlServerGuid?)SqlServerGuid.NewGuid();

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal($"\"{value}\"", json);

        var deserializedValue = JsonSerializer.Deserialize<SqlServerGuid?>(json, options);

        Assert.Equal(value, deserializedValue);
    }

    [Fact]
    public void Should_Persist_Null()
    {
        var options = new JsonSerializerOptions().UseSequentialGuids();

        var value = (SqlServerGuid?)null;

        var json = JsonSerializer.Serialize(value, options);

        Assert.Equal("null", json);

        var deserializedValue = JsonSerializer.Deserialize<SqlServerGuid?>(json, options);

        Assert.Equal(value, deserializedValue);
    }
}