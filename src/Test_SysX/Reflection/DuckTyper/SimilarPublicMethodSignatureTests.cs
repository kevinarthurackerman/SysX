namespace Test_SysX.Reflection.DuckTyper;
using Assert = Xunit.Assert;

public class SimilarPublicMethodSignatureTests
{
    [Fact]
    public void Should_Distinguish_Similar_Methods()
    {
        var value = new Duck();
        var wrapper = SysX.Reflection.DuckTyper.Wrap<IDuck>(value);

        var success1 = wrapper.TryQuack(1, out var result1);

        Assert.True(success1);
        Assert.Equal(1, result1);

        var success2 = wrapper.TryQuack((int?)2, out var result2);

        Assert.True(success2);
        Assert.Equal(2, result2);

        var success3 = wrapper.TryQuack("Quack3", out var result3);

        Assert.True(success3);
        Assert.Equal("Quack3", result3);

        string? result4 = null;
        var success4 = wrapper.TryQuack("Quack4", result4);

        Assert.True(success4);
        Assert.Null(result4);

        Assert.Equal(1, value.MethodOneCallCount);
        Assert.Equal(1, value.MethodTwoCallCount);
        Assert.Equal(1, value.MethodThreeCallCount);
        Assert.Equal(1, value.MethodFourCallCount);
    }

    public interface IDuck
    {
        public bool TryQuack(int value, out int? result);
        public bool TryQuack(int? value, out int? result);
        public bool TryQuack(string? value, out string? result);
        public bool TryQuack(string? value, string? result);
    }

    public class Duck
    {
        public int MethodOneCallCount = 0;
        public int MethodTwoCallCount = 0;
        public int MethodThreeCallCount = 0;
        public int MethodFourCallCount = 0;

        public bool TryQuack(int value, out int? result)
        {
            MethodOneCallCount++;
            result = value;
            return true;
        }

        public bool TryQuack(int? value, out int? result)
        {
            MethodTwoCallCount++;
            result = value;
            return true;
        }

        public bool TryQuack(string? value, out string? result)
        {
            MethodThreeCallCount++;
            result = value;
            return true;
        }

        public bool TryQuack(string? value, string? result)
        {
            MethodFourCallCount++;
            return true;
        }
    }
}