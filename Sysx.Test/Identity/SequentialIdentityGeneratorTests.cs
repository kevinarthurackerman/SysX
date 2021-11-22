using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sysx.Identity;
using Xunit;

namespace Sysx.Test.Identity
{
    public class SequentialIdentityGeneratorTests
    {
        [Fact]
        public void Should_Order_Sequentially()
        {
            var guids = Enumerable.Range(0, 100)
                .Select(x => 
                {
                    Thread.Sleep(1);
                    return new { Order = x, SqlGuid = SequentialIdentityGenerator.Next() };
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
                .Select(x => new { Order = x, SqlGuid = SequentialIdentityGenerator.Next() })
                .ToArray();

            var uniqueGuids = guids.Distinct().ToArray();

            Assert.Equal(guids.Length, uniqueGuids.Length);
        }
    }
}
