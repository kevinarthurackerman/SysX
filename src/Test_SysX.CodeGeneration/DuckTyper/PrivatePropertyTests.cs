namespace Test_SysX.CodeGeneration.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class PrivatePropertyTests
{
    [Fact]
    public void Should_Not_Wrap_Property_To_Private_Property()
    {
        var value = new Duck();
        var wrapper = SysX.CodeGeneration.DuckTyper.Wrap<IDuck>(value, includePrivateMembers: false);

        var getException = Assert.Throws<InvalidOperationException>(() => wrapper.Quack);
        var setException = Assert.Throws<InvalidOperationException>(() => wrapper.Quack = "Quack");

        var expectedExceptionMessage = $"No accessible field or property {typeof(IDuck).GetIdentifier()}.{nameof(IDuck.Quack)} => string found on wrapped value {typeof(Duck).GetIdentifier()}.";

        Assert.Null(value.GetQuack);
        Assert.Equal(expectedExceptionMessage, getException.Message);
        Assert.Equal(expectedExceptionMessage, setException.Message);
    }

    [Fact]
    public void Should_Not_TryWrap_Property_To_Private_Property()
    {
        var value = new Duck();
        var success = SysX.CodeGeneration.DuckTyper.TryWrap<IDuck>(value, out var wrapper, includePrivateMembers: false);

        Assert.False(success);
        Assert.Null(wrapper);
    }

    [Fact]
    public void Should_Wrap_Property_To_Private_Property()
    {
        var value = new Duck();
        var wrapper = SysX.CodeGeneration.DuckTyper.Wrap<IDuck>(value, includePrivateMembers: true);

        wrapper.Quack = "Quack";

        Assert.Equal("Quack", value.GetQuack);
        Assert.Equal("Quack", wrapper.Quack);
    }

    [Fact]
    public void Should_TryWrap_Property_To_Private_Property()
    {
        var value = new Duck();
        var success = SysX.CodeGeneration.DuckTyper.TryWrap<IDuck>(value, out var wrapper, includePrivateMembers: true);

        Assert.True(success);
        Assert.NotNull(wrapper);

        wrapper!.Quack = "Quack";

        Assert.Equal("Quack", value.GetQuack);
        Assert.Equal("Quack", wrapper.Quack);
    }

    public interface IDuck
    {
        public string? Quack { get; set; }
    }

    public class Duck
    {
        private string? Quack { get; set; }

        public string? GetQuack => Quack;
    }
}