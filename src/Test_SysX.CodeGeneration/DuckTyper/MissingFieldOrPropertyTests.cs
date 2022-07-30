namespace Test_SysX.CodeGeneration.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class MissingFieldOrPropertyTests
{
	[Fact]
	public void Should_Not_Wrap_Property_To_Missing_Field_Or_Property()
	{
		var value = new Duck();
		var wrapper = SysX.CodeGeneration.DuckTyper.Wrap<IDuck>(value);

		var getException = Assert.Throws<InvalidOperationException>(() => wrapper.Quack);
		var setException = Assert.Throws<InvalidOperationException>(() => wrapper.Quack = "Quack");

		var expectedExceptionMessage = $"No accessible field or property {typeof(IDuck).GetIdentifier()}.{nameof(IDuck.Quack)} => string found on wrapped value {typeof(Duck).GetIdentifier()}.";

		Assert.Equal(expectedExceptionMessage, getException.Message);
		Assert.Equal(expectedExceptionMessage, setException.Message);

	}

	[Fact]
	public void Should_TryWrap_Property_To_Missing_Field_Or_Property()
	{
		var value = new Duck();
		var success = SysX.CodeGeneration.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

		Assert.False(success);
		Assert.Null(wrapper);
	}

	public interface IDuck
	{
		public string? Quack { get; set; }
	}

	public class Duck
	{

	}
}