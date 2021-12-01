namespace Sysx.Test
{
    using System;
    using Xunit;

    public class TypeExtensionsTests
    {
        [Fact]
        public void Should_Return_Alias()
        {
            var result = typeof(System.Int32).GetAlias();

            Assert.Equal("int", result);
        }

        [Fact]
        public void Should_Return_Null_Alias()
        {
            var result = typeof(System.Console).GetAlias();

            Assert.Null(result);
        }

        [Fact]
        public void Should_Return_Identifier()
        {
            var result = typeof(TestParent.TestChild).GetIdentifier();

            Assert.Equal("Sysx.Test.TypeExtensionsTests.TestParent.TestChild", result);
        }

        [Fact]
        public void Should_Return_Identifier_Alias()
        {
            var result = typeof(System.String).GetIdentifier();

            Assert.Equal("string", result);
        }


        [Fact]
        public void Should_Return_Identifier_For_Null()
        {
            var result = ((Type)null).GetIdentifier();

            Assert.Equal("null", result);
        }

        public class TestParent
        {
            public class TestChild
            {

            }
        }
    }
}
