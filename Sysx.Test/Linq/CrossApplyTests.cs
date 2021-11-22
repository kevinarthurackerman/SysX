using System.Linq;
using Sysx.Linq;
using Xunit;

namespace Sysx.Test.Linq
{
    public class CrossApplyTests
    {
        [Fact]
        public void Should_CrossApply_Self()
        {
            var set = new[] { 1, 2, 3 };

            var actual = set.CrossApply().ToArray();

            var expected = new[]
            {
                new Pair<int>(1,1),
                new Pair<int>(1,2),
                new Pair<int>(1,3),
                new Pair<int>(2,1),
                new Pair<int>(2,2),
                new Pair<int>(2,3),
                new Pair<int>(3,1),
                new Pair<int>(3,2),
                new Pair<int>(3,3)
            };

            for( var i = 0; i < actual.Length; i++)
                Assert.True(actual[i] == expected[i]);
        }

        [Fact]
        public void Should_CrossApply_Other()
        {
            var set1 = new[] { 1, 2, 3 };
            var set2 = new[] { 'a', 'b', };

            var actual = set1.CrossApply(set2).ToArray();

            var expected = new[]
            {
                new Pair<int, char>(1,'a'),
                new Pair<int, char>(1,'b'),
                new Pair<int, char>(2,'a'),
                new Pair<int, char>(2,'b'),
                new Pair<int, char>(3,'a'),
                new Pair<int, char>(3,'b')
            };

            for (var i = 0; i < actual.Length; i++)
                Assert.True(actual[i] == expected[i]);
        }
    }
}
