namespace Test_SysX;
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

        Assert.Equal("Test_SysX.TypeExtensionsTests.TestParent.TestChild", result);
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
    public void Generic_Should_Be_Assignable()
    {
        Assert.True(typeof(TestInheritingGenericClass).IsAssignableToGenericType(typeof(TestGenericClass<>)));
    }

    [Fact]
    public void Open_Generic_Should_Be_Assignable()
    {
        Assert.True(typeof(TestOpenGenericInheritingGenericClass<>).IsAssignableToGenericType(typeof(TestGenericClass<>)));
    }

    [Fact]
    public void Generic_Should_Not_Be_Assignable()
    {
        Assert.False(typeof(object).IsAssignableToGenericType(typeof(TestGenericClass<>)));
    }

    [Fact]
    public void Should_Get_Generic_Implementation()
    {
        Assert.Equal(typeof(TestGenericClass<int>), typeof(TestInheritingGenericClass).GetGenericTypeImplementation(typeof(TestGenericClass<>)));
    }

    [Fact]
    public void Should_Get_Open_Generic_Implementation()
    {
        var typeImplementation = typeof(TestOpenGenericInheritingGenericClass<>).BaseType;
        Assert.Equal(typeImplementation, typeof(TestOpenGenericInheritingGenericClass<>).GetGenericTypeImplementation(typeof(TestGenericClass<>)));
    }

    [Fact]
    public void Should_Not_Get_Generic_Implementation()
    {
        Assert.Null(typeof(object).GetGenericTypeImplementation(typeof(TestGenericClass<>)));
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

    public class TestOpenGenericInheritingGenericClass<T> : TestGenericClass<T> { }
}