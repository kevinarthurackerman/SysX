namespace Test_SysX.CodeGeneration.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class PublicSetPropertyTests
{
	[Fact]
	public void Should_Wrap_Property_To_Public_Property()
	{
		var value = new Duck();
		var wrapper = SysX.CodeGeneration.DuckTyper.Wrap<IDuck>(value);

		wrapper.Quack = "Quack";

		Assert.Equal("Quack", value.QuackValue);
	}

	[Fact]
	public void Should_TryWrap_Property_To_Public_Property()
	{
		var value = new Duck();
		var success = SysX.CodeGeneration.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

		Assert.True(success);
		Assert.NotNull(wrapper);

		wrapper!.Quack = "Quack";

		Assert.Equal("Quack", value.QuackValue);
	}

	public interface IDuck
	{
		public string? Quack { set; }
	}

	public class Duck
	{
		public string? QuackValue;

		public string? Quack { set => QuackValue = value; }
	}
}