﻿namespace Test_SysX.CodeGeneration.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class TryMethodTests
{
	[Fact]
	public void Should_Wrap_Try_Method_To_Try_Method()
	{
		var value = new Duck();
		var wrapper = SysX.CodeGeneration.DuckTyper.Wrap<IDuck>(value);

		var success = wrapper.TryQuack("Quack", out var result);

		Assert.True(success);
		Assert.Equal("Quack", result);
	}

	[Fact]
	public void Should_TryWrap_Try_Method_To_Try_Method()
	{
		var value = new Duck();
		var success = SysX.CodeGeneration.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

		Assert.True(success);
		Assert.NotNull(wrapper);

		var success2 = wrapper!.TryQuack("Quack", out var result);

		Assert.True(success2);
		Assert.Equal("Quack", result);
	}

	public interface IDuck
	{
		public bool TryQuack(string? value, out string? result);
	}

	public class Duck
	{
		public bool TryQuack(string? value, out string? result)
		{
			result = value;
			return true;
		}
	}
}