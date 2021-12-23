namespace Sysx.Test.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class PrivateMethodTests
{
    [Fact]
    public void Should_Not_Wrap_Method_To_Private_Method()
    {
        var value = new Duck();
        var wrapper = Sysx.Reflection.DuckTyper.Wrap<IDuck>(value, includePrivateMembers: false);

        var callException = Assert.Throws<InvalidOperationException>(() => wrapper.Quack("Quack"));

        var expectedExceptionMessage = $"No accessible method {typeof(IDuck).GetIdentifier()}.{nameof(IDuck.Quack)}(string) => string found on wrapped value {typeof(Duck).GetIdentifier()}.";

        Assert.Equal(0, value.QuackCallCount);
        Assert.Equal(expectedExceptionMessage, callException.Message);
    }

    [Fact]
    public void Should_Not_TryWrap_Method_To_Private_Method()
    {
        var value = new Duck();
        var success = Sysx.Reflection.DuckTyper.TryWrap<IDuck>(value, out var wrapper, includePrivateMembers: false);

        Assert.False(success);
        Assert.Null(wrapper);
    }

    [Fact]
    public void Should_Wrap_Method_To_Private_Method()
    {
        var value = new Duck();
        var wrapper = Sysx.Reflection.DuckTyper.Wrap<IDuck>(value, includePrivateMembers: true);

        var result = wrapper.Quack("Quack");

        Assert.Equal("Quack", result);
        Assert.Equal(1, value.QuackCallCount);
    }

    [Fact]
    public void Should_TryWrap_Method_To_Private_Method()
    {
        var value = new Duck();
        var success = Sysx.Reflection.DuckTyper.TryWrap<IDuck>(value, out var wrapper, includePrivateMembers: true);

        Assert.True(success);
        Assert.NotNull(wrapper);

        var result = wrapper!.Quack("Quack");

        Assert.Equal("Quack", result);
        Assert.Equal(1, value.QuackCallCount);
    }

    public interface IDuck
    {
        public string? Quack(string? value);
    }

    public class Duck
    {
        public int QuackCallCount = 0;

        private string? Quack(string? value)
        {
            QuackCallCount++;
            return value;
        }
    }
}