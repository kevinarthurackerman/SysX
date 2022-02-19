namespace Test_SysX.CodeGeneration.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class MissingMethodTests
{
    [Fact]
    public void Should_Not_Wrap_Method_To_Missing_Method()
    {
        var value = new Duck();
        var wrapper = SysX.CodeGeneration.DuckTyper.Wrap<IDuck>(value);

        var callException = Assert.Throws<InvalidOperationException>(() => wrapper.Quack("Quack"));

        var expectedExceptionMessage = $"No accessible method {typeof(IDuck).GetIdentifier()}.{nameof(IDuck.Quack)}(string) => string found on wrapped value {typeof(Duck).GetIdentifier()}.";

        Assert.Equal(expectedExceptionMessage, callException.Message);
    }

    [Fact]
    public void Should_TryWrap_Method_To_Missing_Method()
    {
        var value = new Duck();
        var success = SysX.CodeGeneration.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

        Assert.False(success);
        Assert.Null(wrapper);
    }

    public interface IDuck
    {
        public string? Quack(string value);
    }

    public class Duck
    {

    }
}