﻿namespace Test_SysX.CodeGeneration.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class PublicMethodWithInputAndNoOutputTests
{
	[Fact]
	public void Should_Wrap_Method_To_Public_Method()
	{
		var value = new Duck();
		var wrapper = SysX.CodeGeneration.DuckTyper.Wrap<IDuck>(value);

		wrapper.Quack("Quack");

		Assert.Equal("Quack", value.QuackInputValue);
		Assert.Equal(1, value.QuackCallCount);
	}

	[Fact]
	public void Should_TryWrap_Method_To_Public_Method()
	{
		var value = new Duck();
		var success = SysX.CodeGeneration.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

		Assert.True(success);
		Assert.NotNull(wrapper);

		wrapper!.Quack("Quack");

		Assert.Equal("Quack", value.QuackInputValue);
		Assert.Equal(1, value.QuackCallCount);
	}

	public interface IDuck
	{
		public void Quack(string? value);
	}

	public class Duck
	{
		public int QuackCallCount;
		public string? QuackInputValue;

		public void Quack(string? value)
		{
			QuackCallCount++;
			QuackInputValue = value;
		}
	}
}