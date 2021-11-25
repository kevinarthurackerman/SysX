using System;
using System.Linq;

namespace Sysx.Test.Enums.BaseEnumClass
{
    using Xunit;

    public class SortTests
    {
        [Fact]
        public void Enumerations_should_sort_by_Value_by_default()
        {
            var states = new[] { State.Stopping, State.Off, State.Starting, State.Busy, State.Waiting };

            Array.Sort(states);

            Assert.Equal(5, states.Length);

            Assert.Equal(State.Off, states.ElementAt(0));
            Assert.Equal(State.Starting, states.ElementAt(1));
            Assert.Equal(State.Waiting, states.ElementAt(2));
            Assert.Equal(State.Busy, states.ElementAt(3));
            Assert.Equal(State.Stopping, states.ElementAt(4));
        }
    }
}
