namespace Test_SysX.Enums;
using Assert = Xunit.Assert;

public class FlagsEnumTests
{
	[Fact]
	public void Should_Work_On_Small_Enums()
	{
		var actual = (SmallEnum.A | SmallEnum.B).HasAll(SmallEnum.A);

		Assert.True(actual);
	}

	[Fact]
	public void Should_Check_If_Has()
	{
		var actual = (TestEnum.A | TestEnum.B).HasAll(TestEnum.A);

		Assert.True(actual);
	}

	[Fact]
	public void Should_Check_If_Not_Has()
	{
		var actual = (TestEnum.A | TestEnum.B).HasAll(TestEnum.C);

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

		Assert.True(actual.HasAll(TestEnum.A));
		Assert.True(actual.HasAll(TestEnum.B));
		Assert.True(actual.HasAll(TestEnum.C));
	}

	[Fact]
	public void Should_Add_Idempotent()
	{
		var actual = (TestEnum.A | TestEnum.B).Add(TestEnum.B);

		Assert.True(actual.HasAll(TestEnum.A));
		Assert.True(actual.HasAll(TestEnum.B));
	}

	[Fact]
	public void Should_Remove()
	{
		var actual = (TestEnum.A | TestEnum.B).Remove(TestEnum.B);

		Assert.True(actual.HasAll(TestEnum.A));
		Assert.False(actual.HasAll(TestEnum.B));
	}

	[Fact]
	public void Should_Remove_Idempotent()
	{
		var actual = TestEnum.A.Remove(TestEnum.B);

		Assert.True(actual.HasAll(TestEnum.A));
		Assert.False(actual.HasAll(TestEnum.B));
	}

	[Fact]
	public void Should_Check_If_Zero_Has_No_Value()
	{
		var zero = FlagsEnum.None<TestEnum>();

		Assert.False(zero.HasAll(TestEnum.A));
		Assert.False(zero.HasAll(TestEnum.B));
		Assert.False(zero.HasAll(TestEnum.C));
		Assert.False(zero.HasAll(TestEnum.D));
	}

	[Fact]
	public void Should_Check_If_All_Has_All_Value()
	{
		var all = FlagsEnum.All<TestEnum>();

		Assert.True(all.HasAll(TestEnum.A));
		Assert.True(all.HasAll(TestEnum.B));
		Assert.True(all.HasAll(TestEnum.C));
		Assert.True(all.HasAll(TestEnum.D));
	}

	[Fact]
	public void Should_Expand()
	{
		var value = TestEnum.A | TestEnum.B;

		var result = value.Expand().ToArray();

		Assert.True(result.Length == 2);
		Assert.True(result[0].HasAll(TestEnum.A));
		Assert.True(result[1].HasAll(TestEnum.B));
	}

	[Fact]
	public void Should_Combine()
	{
		var value = FlagsEnum.Combine(TestEnum.A, TestEnum.B);

		Assert.True(value.HasAll(TestEnum.A));
		Assert.True(value.HasAll(TestEnum.B));
		Assert.False(value.HasAll(TestEnum.C));
		Assert.False(value.HasAll(TestEnum.D));
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