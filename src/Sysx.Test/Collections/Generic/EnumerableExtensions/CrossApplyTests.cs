using System.Linq;

namespace Sysx.Test.Collections.Generic.EnumerableExtensions
{
    using Sysx.Collections.Generic;
    using Xunit;

    public class CrossApplyTests
    {
        [Fact]
        public void Should_CrossApply_Self()
        {
            var set = new[] { 1, 2, 3 };

            var actual = set.CrossApply().ToArray();

            var expected = new[]
            {
                (1,1),
                (1,2),
                (1,3),
                (2,1),
                (2,2),
                (2,3),
                (3,1),
                (3,2),
                (3,3)
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
                (1,'a'),
                (1,'b'),
                (2,'a'),
                (2,'b'),
                (3,'a'),
                (3,'b')
            };

            for (var i = 0; i < actual.Length; i++)
                Assert.True(actual[i] == expected[i]);
        }
    }
}
