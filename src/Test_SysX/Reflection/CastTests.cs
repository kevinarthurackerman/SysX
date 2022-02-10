namespace Test_SysX.Reflection;

using System;
using Assert = Xunit.Assert;

public class CastTests
{
    [Fact]
    public void Should_Cast_Primitive_Types()
    {
        Assert.True(Cast<int, long>.CanCast());
        Assert.True(Cast<int>.CanCast<long>());
        Assert.True(Cast.CanCast<int, long>());

        Assert.Equal(1L, Cast<int, long>.Value(1));
        Assert.Equal(1L, Cast<int>.Value<long>(1));
        Assert.Equal(1L, Cast.Value<int, long>(1));
    }

    [Fact]
    public void Should_Cast_Inherited()
    {
        Assert.True(Cast<TestInherited, TestBase>.CanCast());
        Assert.True(Cast<TestInherited>.CanCast<TestBase>());
        Assert.True(Cast.CanCast<TestInherited, TestBase>());

        var testInherited = new TestInherited();

        Assert.Equal(testInherited, Cast<TestInherited, TestBase>.Value(testInherited));
        Assert.Equal(testInherited, Cast<TestInherited>.Value<TestBase>(testInherited));
        Assert.Equal(testInherited, Cast.Value<TestInherited, TestBase>(testInherited));
    }

    [Fact]
    public void Should_Cast_Interface()
    {
        Assert.True(Cast<TestInherited, ITestInterface>.CanCast());
        Assert.True(Cast<TestInherited>.CanCast<ITestInterface>());
        Assert.True(Cast.CanCast<TestInherited, ITestInterface>());

        var testInherited = new TestInherited();

        Assert.Equal(testInherited, Cast<TestInherited, ITestInterface>.Value(testInherited));
        Assert.Equal(testInherited, Cast<TestInherited>.Value<ITestInterface>(testInherited));
        Assert.Equal(testInherited, Cast.Value<TestInherited, ITestInterface>(testInherited));
    }

    [Fact]
    public void Should_Cast_Explicitly()
    {
        Assert.True(Cast<BinaryGuid, Guid>.CanCast());
        Assert.True(Cast<BinaryGuid>.CanCast<Guid>());
        Assert.True(Cast.CanCast<BinaryGuid, Guid>());

        var testInherited = new TestInherited();

        Assert.Equal(Guid.Empty, Cast<BinaryGuid, Guid>.Value(BinaryGuid.Empty));
        Assert.Equal(Guid.Empty, Cast<BinaryGuid>.Value<Guid>(BinaryGuid.Empty));
        Assert.Equal(Guid.Empty, Cast.Value<BinaryGuid, Guid>(BinaryGuid.Empty));
    }

    public interface ITestInterface { }

    public class TestBase : ITestInterface { }

    public class TestInherited : TestBase { }
}
