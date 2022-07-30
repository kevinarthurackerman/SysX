namespace Test_SysX;
using Assert = Xunit.Assert;

public class OperatorTests
{
	[Fact]
	public void Should_Throw_InvalidOperationException_When_Unary_Operation_Does_Not_Exist()
	{
		Assert.Throws<InvalidOperationException>(() => Operator<float>.And(1, 1));
	}

	[Fact]
	public void Should_Throw_InvalidOperationException_When_Binary_Operation_Does_Not_Exist()
	{
		Assert.Throws<InvalidOperationException>(() => Operator<byte>.Add(1, 1));
	}

	[Fact]
	public void Should_Throw_InvalidCastException_When_Unary_Operation_Result_Is_Invaid_Cast()
	{
		Assert.Throws<InvalidCastException>(() => Operator<float, bool>.Decrement(1));
	}

	[Fact]
	public void Should_Throw_InvalidCastException_When_Binary_Operation_Result_Is_Invaid_Cast()
	{
		Assert.Throws<InvalidCastException>(() => Operator<float, float, bool>.Add(1, 1));
	}

	[Fact]
	public void Should_Execute_Unary_Operation()
	{
		var actual = Operator<int, int>.Increment(1);
		Assert.Equal(2, actual);
	}

	[Fact]
	public void Should_Execute_Binary_Operation()
	{
		var actual = Operator<int, int, long>.Add(1, 1);
		Assert.Equal(2, actual);
	}
}