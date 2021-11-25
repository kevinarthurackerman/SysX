using System;

namespace Sysx.Test.Enums.BaseEnumClass
{
    using Xunit;

    public class ParseTests
    {
        private const string BadDisplayName = "Blarg";
        private const int BadValue = -1337;

        [Fact]
        public void Parse_with_good_DisplayName_should_return_expected_Enumeration()
        {
            Assert.Equal(Color.Parse("Red"), Color.Red);
        }

        [Fact]
        public void FromValue_with_good_Value_should_return_expected_Enumeration()
        {
            Assert.Equal(Color.ParseValue(2), Color.Blue);
        }

        [Fact]
        public void Parse_with_bad_DisplayName_should_throw_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Color.Parse(BadDisplayName));
        }

        [Fact]
        public void FromValue_with_bad_Value_should_throw_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Color.ParseValue(BadValue));
        }

        [Fact]
        public void TryParse_with_good_DisplayName_should_return_expected_Enumeration()
        {
            Assert.True(Color.TryParse("Blue", out var color));
            Assert.Equal(Color.Blue, color);
        }

        [Fact]
        public void TryParse_with_good_Value_should_return_expected_Enumeration()
        {
            Assert.True(Color.TryParseValue(1, out var color));
            Assert.Equal(Color.Red, color);
        }

        [Fact]
        public void TryParse_with_bad_DisplayName_should_not_throw_ArgumentException()
        {
            var parseResult = Color.TryParse(BadDisplayName, out var color);
            Assert.False(parseResult);
            Assert.Null(color);
        }

        [Fact]
        public void TryParse_with_bad_Value_should_not_throw_ArgumentException()
        {
            var parseResult = Color.TryParseValue(BadValue, out var color);
            Assert.False(parseResult);
            Assert.Null(color);
        }
    }
}
