namespace Test_SysX.Enums.BaseEnumeration;
using Assert = Xunit.Assert;

public class EqualityTests
{
    [Fact]
    public void Equals_operator_should_return_true_for_same_value()
    {
        var first = Color.Red;
        var second = Color.Red;

        Assert.True(first == second);
    }

    [Fact]
    public void NotEquals_operator_should_return_true_for_different_values()
    {
        var first = Color.Red;
        var second = Color.Blue;

        Assert.True(first != second);
    }

    [Fact]
    public void Equals_operator_should_return_false_for_different_values()
    {
        var first = Color.Blue;
        var second = Color.Red;

        Assert.False(first == second);
    }

    [Fact]
    public void NotEquals_operator_should_return_false_for_same_values()
    {
        var first = Color.Blue;
        var second = Color.Blue;

        Assert.False(first != second);
    }
}