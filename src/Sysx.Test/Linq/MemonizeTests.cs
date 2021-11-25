using System.Linq;
using Sysx.Linq;

namespace Sysx.Test.Linq
{
    using Xunit;

    public class MemonizeTests
    {
        [Fact]
        public void Should_Cache()
        {
            var calledCount = 0;

            var testEnumerable = Enumerable.Range(0, 1)
                .Select(x => calledCount += 1)
                .Memoize();

            Assert.Equal(0, calledCount);

            var firstCall = testEnumerable.ToArray();
            var secondCall = testEnumerable.ToArray();
            var thirdCall = testEnumerable.ToArray();

            Assert.Equal(1, calledCount);
        }

        [Fact]
        public void Should_Not_Cache()
        {
            var calledCount = 0;

            var testEnumerable = Enumerable.Range(0, 1)
                .Select(x => calledCount += 1);

            Assert.Equal(0, calledCount);

            var firstCall = testEnumerable.ToArray();
            var secondCall = testEnumerable.ToArray();
            var thirdCall = testEnumerable.ToArray();

            Assert.Equal(3, calledCount);
        }
    }
}
