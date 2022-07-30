namespace Test_SysX.Enums.BaseEnumeration;
using Assert = Xunit.Assert;

public class CompareTests
{
	[Fact]
	public void CompareTo_should_use_Value_of_Enumeration()
	{
		var red = Color.Red;
		var blue = Color.Blue;

		Assert.Equal(red.CompareTo(blue), red.Value.CompareTo(blue.Value));
	}

	[Fact]
	public void CompareTo_on_instance_should_compare_greater_than_null()
	{
		var red = Color.Red;

		Assert.True(red.CompareTo(default) > 0);
	}

	[Fact]
	public void CompareTo_should_return_zero_for_same_instance_comparisons()
	{
		var first = Color.Blue;
		var second = Color.Blue;

		Assert.Equal(0, first.CompareTo(second));
	}
}
