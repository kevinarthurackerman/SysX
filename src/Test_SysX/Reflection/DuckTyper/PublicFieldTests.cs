﻿namespace Test_SysX.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class PublicFieldTests
{
    [Fact]
    public void Should_Wrap_Property_To_Public_Field()
    {
        var value = new Duck();
        var wrapper = SysX.Reflection.DuckTyper.Wrap<IDuck>(value);

        wrapper.Quack = "Quack";

        Assert.Equal("Quack", value.Quack);
        Assert.Equal("Quack", wrapper.Quack);
    }

    [Fact]
    public void Should_TryWrap_Property_To_Public_Field()
    {
        var value = new Duck();
        var success = SysX.Reflection.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

        Assert.True(success);
        Assert.NotNull(wrapper);

        wrapper!.Quack = "Quack";

        Assert.Equal("Quack", value.Quack);
        Assert.Equal("Quack", wrapper.Quack);
    }

    public interface IDuck
    {
        public string? Quack { get; set; }
    }

    public class Duck
    {
        public string? Quack;
    }
}