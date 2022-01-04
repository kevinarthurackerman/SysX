namespace Test_Sysx.Text.Json.Serialization.Identifiers;
using Assert = Assert;

public class IdentifierJsonConverterTests
{
    [Fact]
    public void Should_Persist_Values()
    {
        var options = new JsonSerializerOptions().UseIdentifiers();

        var testObject = new GuidPropertiesModel
        {
            Guid = Guid.NewGuid(),
            BinaryGuid = BinaryGuid.NewGuid(),
            StringGuid = StringGuid.NewGuid(),
            SqlServerGuid = SqlServerGuid.NewGuid(),
            NullableGuid = Guid.NewGuid(),
            NullableBinaryGuid = BinaryGuid.NewGuid(),
            NullableStringGuid = StringGuid.NewGuid(),
            NullableSqlServerGuid = SqlServerGuid.NewGuid()
        };

        var json = JsonSerializer.Serialize(testObject, options);

        Assert.Equal(CreateExpectedJson(testObject), json);

        var persistedTestObject = JsonSerializer.Deserialize<GuidPropertiesModel>(json, options);

        Assert.NotNull(persistedTestObject);
        Assert.Equal(testObject.Guid, persistedTestObject!.Guid);
        Assert.Equal(testObject.BinaryGuid, persistedTestObject.BinaryGuid);
        Assert.Equal(testObject.StringGuid, persistedTestObject.StringGuid);
        Assert.Equal(testObject.SqlServerGuid, persistedTestObject.SqlServerGuid);
        Assert.Equal(testObject.NullableGuid, persistedTestObject.NullableGuid);
        Assert.Equal(testObject.NullableBinaryGuid, persistedTestObject.NullableBinaryGuid);
        Assert.Equal(testObject.NullableStringGuid, persistedTestObject.NullableStringGuid);
        Assert.Equal(testObject.NullableSqlServerGuid, persistedTestObject.NullableSqlServerGuid);
    }

    [Fact]
    public void Should_Persist_Empty_Values()
    {
        var options = new JsonSerializerOptions().UseIdentifiers();

        var testObject = new GuidPropertiesModel
        {
            Guid = default,
            BinaryGuid = default,
            StringGuid = default,
            SqlServerGuid = default,
            NullableGuid = default,
            NullableBinaryGuid = default,
            NullableStringGuid = default,
            NullableSqlServerGuid = default
        };

        var json = JsonSerializer.Serialize(testObject, options);

        Assert.Equal(CreateExpectedJson(testObject), json);

        var persistedTestObject = JsonSerializer.Deserialize<GuidPropertiesModel>(json, options);

        Assert.NotNull(persistedTestObject);
        Assert.Equal(testObject.Guid, persistedTestObject!.Guid);
        Assert.Equal(testObject.BinaryGuid, persistedTestObject.BinaryGuid);
        Assert.Equal(testObject.StringGuid, persistedTestObject.StringGuid);
        Assert.Equal(testObject.SqlServerGuid, persistedTestObject.SqlServerGuid);
        Assert.Equal(testObject.NullableGuid, persistedTestObject.NullableGuid);
        Assert.Equal(testObject.NullableBinaryGuid, persistedTestObject.NullableBinaryGuid);
        Assert.Equal(testObject.NullableStringGuid, persistedTestObject.NullableStringGuid);
        Assert.Equal(testObject.NullableSqlServerGuid, persistedTestObject.NullableSqlServerGuid);
    }

    private static string CreateExpectedJson(GuidPropertiesModel model) =>
        $"{{\"Guid\":{WrapJsonValue(model.Guid)},\"BinaryGuid\":{WrapJsonValue(model.BinaryGuid)},\"StringGuid\":{WrapJsonValue(model.StringGuid)},\"SqlServerGuid\":{WrapJsonValue(model.SqlServerGuid)},\"NullableGuid\":{WrapJsonValue(model.NullableGuid)},\"NullableBinaryGuid\":{WrapJsonValue(model.NullableBinaryGuid)},\"NullableStringGuid\":{WrapJsonValue(model.NullableStringGuid)},\"NullableSqlServerGuid\":{WrapJsonValue(model.NullableSqlServerGuid)}}}";

    private static string WrapJsonValue(object? value) =>
        value == null ? "null" : $"\"{value}\"";

    public class GuidPropertiesModel
    {
        public Guid Guid { get; set; }
        public BinaryGuid BinaryGuid { get; set; }
        public StringGuid StringGuid { get; set; }
        public SqlServerGuid SqlServerGuid { get; set; }
        public Guid? NullableGuid { get; set; }
        public BinaryGuid? NullableBinaryGuid { get; set; }
        public StringGuid? NullableStringGuid { get; set; }
        public SqlServerGuid? NullableSqlServerGuid { get; set; }
    }
}