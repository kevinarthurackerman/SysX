namespace Test_Sysx.Text.Json.Serialization.Enums;
using Assert = Assert;

public class EnumerationsByValueWithCustomTypesTests
{
    [Fact]
    public void Should_Persist_Values()
    {
        var options = new JsonSerializerOptions().UseEnumerationsByValue().UseSequentialGuids();

        var testObject = new EnumerationPropertiesModel
        {
            Null = Animal.Null,
            Lion = Mammal.Lion,
            Snake = Reptile.Snake,
            LionAnimal = Mammal.Lion,
            SnakeAnimal = Reptile.Snake
        };

        var json = JsonSerializer.Serialize(testObject, options);

        Assert.Equal(CreateExpectedJson(testObject), json);

        var persistedTestObject = JsonSerializer.Deserialize<EnumerationPropertiesModel>(json, options);

        Assert.NotNull(persistedTestObject);
        Assert.Equal(testObject.Null, persistedTestObject!.Null);
        Assert.Equal(testObject.Lion, persistedTestObject.Lion);
        Assert.Equal(testObject.Snake, persistedTestObject.Snake);
        Assert.Equal(testObject.LionAnimal, persistedTestObject.LionAnimal);
        Assert.Equal(testObject.SnakeAnimal, persistedTestObject.SnakeAnimal);
    }

    [Fact]
    public void Should_Persist_Empty_Values()
    {
        var options = new JsonSerializerOptions().UseEnumerationsByValue().UseSequentialGuids();

        var testObject = new EnumerationPropertiesModel
        {
            Null = default,
            Lion = default,
            Snake = default,
            LionAnimal = default,
            SnakeAnimal = default
        };

        var json = JsonSerializer.Serialize(testObject, options);

        Assert.Equal(CreateExpectedJson(testObject), json);

        var persistedTestObject = JsonSerializer.Deserialize<EnumerationPropertiesModel>(json, options);

        Assert.NotNull(persistedTestObject);
        Assert.Equal(testObject.Null, persistedTestObject!.Null);
        Assert.Equal(testObject.Lion, persistedTestObject.Lion);
        Assert.Equal(testObject.Snake, persistedTestObject.Snake);
        Assert.Equal(testObject.LionAnimal, persistedTestObject.LionAnimal);
        Assert.Equal(testObject.SnakeAnimal, persistedTestObject.SnakeAnimal);
    }

    private static string CreateExpectedJson(EnumerationPropertiesModel model) =>
        $"{{\"Null\":{WrapJsonValue(model.Null)},\"Lion\":{WrapJsonValue(model.Lion)},\"Snake\":{WrapJsonValue(model.Snake)},\"LionAnimal\":{WrapJsonValue(model.LionAnimal)},\"SnakeAnimal\":{WrapJsonValue(model.SnakeAnimal)}}}";

    private static string WrapJsonValue(Animal? value) =>
        value == null ? "null" : $"\"{value.Value}\"";

    public class EnumerationPropertiesModel
    {
        public Animal? Null { get; set; }
        public Mammal? Lion { get; set; }
        public Reptile? Snake { get; set; }
        public Animal? LionAnimal { get; set; }
        public Animal? SnakeAnimal { get; set; }
    }

    public class Animal : BaseEnumeration<Animal, BinaryGuid>
    {
        public static readonly Animal Null = new(BinaryGuid.NewGuid(), "Null");

        protected Animal(BinaryGuid value, string displayName) : base(value, displayName) { }
    }

    public class Mammal : Animal
    {
        public static readonly Mammal Lion = new(BinaryGuid.NewGuid(), "Lion");
        public static readonly Mammal Tiger = new(BinaryGuid.NewGuid(), "Tiger");
        public static readonly Mammal Bear = new(BinaryGuid.NewGuid(), "Bear");

        protected Mammal(BinaryGuid value, string displayName) : base(value, displayName) { }
    }

    public class Reptile : Animal
    {
        public static readonly Reptile Snake = new(BinaryGuid.NewGuid(), "Snake");
        public static readonly Reptile Turtle = new(BinaryGuid.NewGuid(), "Turtle");

        protected Reptile(BinaryGuid value, string displayName) : base(value, displayName) { }
    }
}