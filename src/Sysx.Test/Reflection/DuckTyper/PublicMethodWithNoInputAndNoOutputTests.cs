namespace Sysx.Test.Reflection.DuckTyper
{
    using Xunit;

    public class PublicMethodWithNoInputAndNoOutputTests
    {
        [Fact]
        public void Should_Wrap_Method_To_Public_Method()
        {
            var value = new Duck();
            var wrapper = Sysx.Reflection.DuckTyper.Wrap<IDuck>(value);

            wrapper.Quack();

            Assert.Equal(1, value.QuackCallCount);
        }

        [Fact]
        public void Should_TryWrap_Method_To_Public_Method()
        {
            var value = new Duck();
            var success = Sysx.Reflection.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

            Assert.True(success);
            Assert.NotNull(wrapper);

            wrapper!.Quack();

            Assert.Equal(1, value.QuackCallCount);
        }

        public interface IDuck
        {
            public void Quack();
        }

        public class Duck
        {
            public int QuackCallCount;

            public void Quack()
            {
                QuackCallCount++;
            }
        }
    }
}
