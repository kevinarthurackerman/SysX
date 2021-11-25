using System.Linq;
using System.Threading;
using Sysx.Identity;

namespace Sysx.Test.Identity
{
    using Xunit;

    public class SequentialGuidTests
    {
        [Fact]
        public void Should_Order_Sequentially()
        {
            var guids = Enumerable.Range(0, 100)
                .Select(x => 
                {
                    Thread.Sleep(1);
                    return new { Order = x, SqlGuid = SequentialGuid.Next() };
                })
                .ToArray();

            var ordered = guids.OrderBy(x => x.SqlGuid).ToArray();

            for(var i = 0; i < guids.Length; i++)
                Assert.Equal(guids[i], ordered[i]);
        }

        [Fact]
        public void Should_Generate_Unique_Values()
        {
            var guids = Enumerable.Range(0, 1000)
                .Select(x => new { Order = x, SqlGuid = SequentialGuid.Next() })
                .ToArray();

            var uniqueGuids = guids.Distinct().ToArray();

            Assert.Equal(guids.Length, uniqueGuids.Length);
        }
    }
}
