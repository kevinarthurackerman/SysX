using System;

namespace Sysx.Test.Reflection.DuckTyper
{
    using Xunit;

    public class MissingTryMethodTests
    {
        [Fact]
        public void Should_Not_Wrap_Try_Method_To_Missing_Try_Method()
        {
            var value = new Duck();
            var wrapper = Sysx.Reflection.DuckTyper.Wrap<IDuck>(value);

            string? result = null;
            var callException = Assert.Throws<InvalidOperationException>(() => wrapper.TryQuack("Quack", out result));

            var expectedExceptionMessage = $"No accessible method {typeof(IDuck).GetIdentifier()}.{nameof(IDuck.TryQuack)}(string, out string) => bool found on wrapped value {typeof(Duck).GetIdentifier()}.";

            Assert.Null(result);
            Assert.Equal(expectedExceptionMessage, callException.Message);
        }

        [Fact]
        public void Should_TryWrap_Try_Method_To_Missing_Try_Method()
        {
            var value = new Duck();
            var success = Sysx.Reflection.DuckTyper.TryWrap<IDuck>(value, out var wrapper);

            Assert.False(success);
            Assert.Null(wrapper);
        }

        public interface IDuck
        {
            public bool TryQuack(string? value, out string? result);
        }

        public class Duck
        {
        }
    }
}
