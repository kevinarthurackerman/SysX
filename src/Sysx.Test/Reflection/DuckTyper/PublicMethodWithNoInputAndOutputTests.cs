namespace Sysx.Test.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class PublicMethodWithNoInputAndOutputTests
{
    [Fact]
    public void Should_Wrap_Method_To_Public_Method()
    {
        var value = new Duck();
        var wrapper = Sysx.Reflection.DuckTyper.Wrap<IDuck>(value);

        var result = wrapper.Quack();

        Assert.Equal("Quack", result);
        Assert.Equal(1, value.QuackCallCount);
    }

    [Fact]
    public void Should_TryWrap_Method_To_Public_Method()
    {
        var value = new Duck();
        var success = Sysx.Reflection.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

        Assert.True(success);
        Assert.NotNull(wrapper);

        var result = wrapper!.Quack();

        Assert.Equal("Quack", result);
        Assert.Equal(1, value.QuackCallCount);
    }

    public interface IDuck
    {
        public string? Quack();
    }

    public class Duck
    {
        public int QuackCallCount = 0;

        public string? Quack()
        {
            QuackCallCount++;
            return "Quack";
        }
    }
}