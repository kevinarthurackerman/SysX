using System.Runtime.CompilerServices;

namespace Sysx.Test
{
    using Xunit;

    public class AssertTests
    {
        [Fact]
        public void Should_Not_Throw_When_True()
        {
            var exception = Record.Exception(() => Sysx.Assert.That(true));

            Assert.Null(exception);
        }

#if ASSERTIONS
        [Fact]
        public void Should_Throw_When_False()
        {
            Assert.Throws<ContractException>(() => Sysx.Assert.That(false));
        }

        [Fact]
        public void Should_Throw_With_Message()
        {
            var varA = 1;
            var varB = 2;

            var lineNumber = GetLineNumber() + 1;
            var exception = Record.Exception(() => Sysx.Assert.That(varA == varB));

            Assert.NotNull(exception);
            Assert.Equal(typeof(ContractException), exception.GetType());

    #if NET5_0 || NETCOREAPP3_1
            Assert.Equal($@"Condition 'varA == varB' failed at C:\Users\User\source\repos\Sysx\src\Sysx.Test\AssertTests.cs line {lineNumber}: Should_Throw_With_Message", exception.Message);
    #endif
    #if NET48
            Assert.Equal($@"Condition failed at C:\Users\User\source\repos\Sysx\src\Sysx.Test\AssertTests.cs line {lineNumber}: Should_Throw_With_Message", exception.Message);
    #endif
        }

        [Fact]
        public void Should_Format_Message()
        {
            var lineNumber = GetLineNumber() + 1;
            var exception = Record.Exception(() => Sysx.Assert.That(false, x => $"Line: {x.LineNumber}"));

            Assert.NotNull(exception);
            Assert.Equal(typeof(ContractException), exception.GetType());

            Assert.Equal($"Line: {lineNumber}", exception.Message);
        }
#else

        [Fact]
        public void Should_Not_Throw_When_False()
        {
            var exception = Record.Exception(() => Sysx.Assert.That(false));

            Assert.Null(exception);
        }
#endif

        private static int? GetLineNumber([CallerLineNumber] int? lineNumber = null) => lineNumber;
    }
}