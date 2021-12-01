namespace Sysx.Test.Reflection.DuckTyper
{
    using Xunit;

    public class PublicGetPropertyTests
    {
        [Fact]
        public void Should_Wrap_Property_To_Public_Property()
        {
            var value = new Duck();
            var wrapper = Sysx.Reflection.DuckTyper.Wrap<IDuck>(value);

            Assert.Equal("Quack", value.Quack);
            Assert.Equal("Quack", wrapper.Quack);
        }

        [Fact]
        public void Should_TryWrap_Property_To_Public_Property()
        {
            var value = new Duck();
            var success = Sysx.Reflection.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

            Assert.True(success);
            Assert.NotNull(wrapper);

            Assert.Equal("Quack", value.Quack);
            Assert.Equal("Quack", wrapper.Quack);
        }

        public interface IDuck
        {
            public string? Quack { get; }
        }

        public class Duck
        {
            public string? Quack { get; } = "Quack";
        }
    }
}
