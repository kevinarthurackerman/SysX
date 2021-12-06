namespace Sysx.Test.Math;
using Assert = Xunit.Assert;

public class FlagsEnumTests
{
    [Fact]
    public void Should_Work_On_Small_Enums()
    {
        var actual = (SmallEnum.A | SmallEnum.B).Has(SmallEnum.A);

        Assert.True(actual);
    }

    [Fact]
    public void Should_Check_If_Has()
    {
        var actual = (TestEnum.A | TestEnum.B).Has(TestEnum.A);

        Assert.True(actual);
    }

    [Fact]
    public void Should_Check_If_Not_Has()
    {
        var actual = (TestEnum.A | TestEnum.B).Has(TestEnum.C);

        Assert.False(actual);
    }

    [Fact]
    public void Should_Check_If_Has_Any()
    {
        var actual = (TestEnum.A | TestEnum.B).HasAny(TestEnum.A | TestEnum.C);

        Assert.True(actual);
    }

    [Fact]
    public void Should_Check_If_Not_Has_Any()
    {
        var actual = (TestEnum.A | TestEnum.B).HasAny(TestEnum.C | TestEnum.D);

        Assert.False(actual);
    }

    [Fact]
    public void Should_Add()
    {
        var actual = (TestEnum.A | TestEnum.B).Add(TestEnum.C);

        Assert.True(actual.Has(TestEnum.A));
        Assert.True(actual.Has(TestEnum.B));
        Assert.True(actual.Has(TestEnum.C));
    }

    [Fact]
    public void Should_Add_Idempotent()
    {
        var actual = (TestEnum.A | TestEnum.B).Add( TestEnum.B);

        Assert.True(actual.Has(TestEnum.A));
        Assert.True(actual.Has(TestEnum.B));
    }

    [Fact]
    public void Should_Remove()
    {
        var actual = (TestEnum.A | TestEnum.B).Remove(TestEnum.B);

        Assert.True(actual.Has(TestEnum.A));
        Assert.False(actual.Has(TestEnum.B));
    }

    [Fact]
    public void Should_Remove_Idempotent()
    {
        var actual = TestEnum.A.Remove(TestEnum.B);

        Assert.True(actual.Has(TestEnum.A));
        Assert.False(actual.Has(TestEnum.B));
    }

    [Fact]
    public void Should_Check_If_Zero_Has_No_Value()
    {
        var zero = FlagsEnum.None<TestEnum>();

        Assert.False(zero.Has(TestEnum.A));
        Assert.False(zero.Has(TestEnum.B));
        Assert.False(zero.Has(TestEnum.C));
        Assert.False(zero.Has(TestEnum.D));
    }

    [Fact]
    public void Should_Check_If_All_Has_All_Value()
    {
        var all = FlagsEnum.All<TestEnum>();

        Assert.True(all.Has(TestEnum.A));
        Assert.True(all.Has(TestEnum.B));
        Assert.True(all.Has(TestEnum.C));
        Assert.True(all.Has(TestEnum.D));
    }

    [Fact]
    public void Should_Expand()
    {
        var value = TestEnum.A | TestEnum.B;

        var result = value.Expand().ToArray();

        Assert.True(result.Length == 2);
        Assert.True(result[0].Has(TestEnum.A));
        Assert.True(result[1].Has(TestEnum.B));
    }

    [Fact]
    public void Should_Combine()
    {
        var value = FlagsEnum.Combine(TestEnum.A, TestEnum.B);

        Assert.True(value.Has(TestEnum.A));
        Assert.True(value.Has(TestEnum.B));
        Assert.False(value.Has(TestEnum.C));
        Assert.False(value.Has(TestEnum.D));
    }

    private enum TestEnum
    {
        A = 1,
        B = 1 << 1,
        C = 1 << 2,
        D = 1 << 3
    }

    private enum SmallEnum : byte
    {
        A = 1,
        B = 1 << 1,
        C = 1 << 2,
        D = 1 << 3
    }
}