namespace Test_SysX.CodeGeneration.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class PublicReadonlyFieldTests
{
	[Fact]
	public void Should_Wrap_Property_To_Public_Field()
	{
		var value = new Duck();
		var wrapper = SysX.CodeGeneration.DuckTyper.Wrap<IDuck>(value);

		Assert.Equal("Quack", value.Quack);
		Assert.Equal("Quack", wrapper.Quack);
	}

	[Fact]
	public void Should_TryWrap_Property_To_Public_Field()
	{
		var value = new Duck();
		var success = SysX.CodeGeneration.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

		Assert.True(success);
		Assert.NotNull(wrapper);

		Assert.Equal("Quack", value.Quack);
		Assert.Equal("Quack", wrapper!.Quack);
	}

	public interface IDuck
	{
		public string? Quack { get; }
	}

	public class Duck
	{
		public readonly string Quack = "Quack";
	}
}