namespace Test_SysX.Reflection;
using Assert = Xunit.Assert;

public class MethodInfoExtensionsTests
{
	[Fact]
	public void Should_Match_Method()
	{
		var method = typeof(TestClass).GetMethod("Method")!;
		var compareToMethod = typeof(MatchingMethodClass).GetMethod("Method")!;

		Assert.True(method.MatchesSignature(compareToMethod));
	}

	[Fact]
	public void Should_Not_Match_Method_With_Different_Name()
	{
		var method = typeof(TestClass).GetMethod("Method")!;
		var compareToMethod = typeof(NonMatchingMethodClass1).GetMethod("DifferentMethod")!;

		Assert.False(method.MatchesSignature(compareToMethod));
	}

	[Fact]
	public void Should_Not_Match_Method_With_Different_Parameters()
	{
		var method = typeof(TestClass).GetMethod("Method")!;
		var compareToMethod = typeof(NonMatchingMethodClass2).GetMethod("Method")!;

		Assert.False(method.MatchesSignature(compareToMethod));
	}

	[Fact]
	public void Should_Not_Match_Method_With_Different_Return_Type()
	{
		var method = typeof(TestClass).GetMethod("Method")!;
		var compareToMethod = typeof(NonMatchingMethodClass3).GetMethod("Method")!;

		Assert.False(method.MatchesSignature(compareToMethod));
	}

	public class TestClass
	{
		public string Method(int ivalue, double dvalue) { return string.Empty; }
	}

	public class MatchingMethodClass
	{
		public string Method(int intval, double doubleval) { return "Test"; }
	}

	public class NonMatchingMethodClass1
	{
		public string DifferentMethod(int intval, double doubleval) { return "Test"; }
	}

	public class NonMatchingMethodClass2
	{
		public string Method(int intval, string strval) { return "Test"; }
	}

	public class NonMatchingMethodClass3
	{
		public int Method(int ivalue, double dvalue) { return 0; }
	}
}