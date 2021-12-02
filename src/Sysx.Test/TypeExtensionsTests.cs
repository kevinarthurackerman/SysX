namespace Sysx.Test;
using Assert = Xunit.Assert;

public class TypeExtensionsTests
{
    [Fact]
    public void Should_Return_Alias()
    {
        var result = typeof(System.Int32).GetAlias();

        Assert.Equal("int", result);
    }

    [Fact]
    public void Should_Return_Null_Alias()
    {
        var result = typeof(System.Console).GetAlias();

        Assert.Null(result);
    }

    [Fact]
    public void Should_Return_Identifier()
    {
        var result = typeof(TestParent.TestChild).GetIdentifier();

        Assert.Equal("Sysx.Test.TypeExtensionsTests.TestParent.TestChild", result);
    }

    [Fact]
    public void Should_Return_Identifier_Alias()
    {
        var result = typeof(System.String).GetIdentifier();

        Assert.Equal("string", result);
    }


    [Fact]
    public void Should_Return_Identifier_For_Null()
    {
        Type? type = null;

        var result = type.GetIdentifier();

        Assert.Equal("null", result);
    }

    [Fact]
    public void Should_Check_Nullability()
    {
        Assert.True(typeof(string).IsNullable());
        Assert.True(typeof(object).IsNullable());
        Assert.True(typeof(TestParent).IsNullable());
        Assert.True(typeof(int?).IsNullable());
        Assert.False(typeof(bool).IsNullable());
        Assert.False(typeof(int).IsNullable());
        Assert.False(typeof(TestStruct).IsNullable());
        Assert.Throws<ArgumentNullException>(() =>
        {
            Type? type = null;
            type!.IsNullable();
        });
    }

    public class TestParent
    {
        public class TestChild
        {

        }
    }

    public struct TestStruct { }
}