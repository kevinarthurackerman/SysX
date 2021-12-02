using System;
using System.Linq;
using Sysx.Identity;

namespace Sysx.Test.Identity
{
    using Xunit;

    public class SequentialGuidTests
    {
        [Fact]
        public void Should_Order_Sequentially()
        {
            var time = new DateTime(2020, 1, 1);

            var options = new SequentialGuid.Options
            {
                GetNow = () =>
                {
                    time = time.AddMilliseconds(1);
                    return time;
                }
            };

            var guids = Enumerable.Range(0, 1000)
                .Select(x => SequentialGuid.Next(options))
                .ToArray();

            var ordered = guids.OrderBy(x => x).ToArray();

            for(var i = 0; i < guids.Length; i++)
                Assert.Equal(guids[i], ordered[i]);
        }

        [Fact]
        public void Should_Generate_Unique_Values()
        {
            var guids = Enumerable.Range(0, 1000)
                .Select(_ => SequentialGuid.Next())
                .ToArray();

            var uniqueGuids = guids.Distinct().ToArray();

            Assert.Equal(guids.Length, uniqueGuids.Length);
        }
    }
}
