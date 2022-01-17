namespace Test_Sysx;
using Assert = Xunit.Assert;

public class TypeExtensionsTests
{
    [Fact]
    public void Should_Return_Alias()
    {
        var result = typeof(int).GetAlias();

        Assert.Equal("int", result);
    }

    [Fact]
    public void Should_Return_Null_Alias()
    {
        var result = typeof(Console).GetAlias();

        Assert.Null(result);
    }

    [Fact]
    public void Should_Return_Identifier()
    {
        var result = typeof(TestParent.TestChild).GetIdentifier();

        Assert.Equal("Test_Sysx.TypeExtensionsTests.TestParent.TestChild", result);
    }

    [Fact]
    public void Should_Return_Identifier_Alias()
    {
        var result = typeof(string).GetIdentifier();

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

    [Fact]
    public void Should_Be_Assignable()
    {
        Assert.True(typeof(TestInheritingGenericClass).IsAssignableToGenericType(typeof(TestGenericClass<>)));
    }

    [Fact]
    public void Should_Not_Be_Assignable()
    {
        Assert.False(typeof(object).IsAssignableToGenericType(typeof(TestGenericClass<>)));
    }

    [Fact]
    public void Should_Get_Generic_Argument()
    {
        Assert.Equal(typeof(int), typeof(TestInheritingGenericClass).GetGenericTypeArgument(typeof(TestGenericClass<>)));
    }

    [Fact]
    public void Should_Not_Get_Generic_Argument()
    {
        Assert.Null(typeof(object).GetGenericTypeArgument(typeof(TestGenericClass<>)));
    }

    public class TestParent
    {
        public class TestChild
        {

        }
    }

    public struct TestStruct { }

    public class TestGenericClass<T> { }

    public class TestInheritingGenericClass : TestGenericClass<int> { }
}