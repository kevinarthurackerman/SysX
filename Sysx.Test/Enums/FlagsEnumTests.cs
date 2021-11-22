using System.Linq;
using Sysx.Enums;
using Xunit;

namespace Sysx.Test.Math
{
    public class FlagsEnumTests
    {
        [Fact]
        public void Should_Work_On_Small_Enums()
        {
            var actual = FlagsEnum.Has(SmallEnum.A | SmallEnum.B, SmallEnum.A);

            Assert.True(actual);
        }

        [Fact]
        public void Should_Check_If_Has()
        {
            var actual = FlagsEnum.Has(TestEnum.A | TestEnum.B, TestEnum.A);

            Assert.True(actual);
        }

        [Fact]
        public void Should_Check_If_Not_Has()
        {
            var actual = FlagsEnum.Has(TestEnum.A | TestEnum.B, TestEnum.C);

            Assert.False(actual);
        }

        [Fact]
        public void Should_Check_If_Has_Any()
        {
            var actual = FlagsEnum.HasAny(TestEnum.A | TestEnum.B, TestEnum.A | TestEnum.C);

            Assert.True(actual);
        }

        [Fact]
        public void Should_Check_If_Not_Has_Any()
        {
            var actual = FlagsEnum.HasAny(TestEnum.A | TestEnum.B, TestEnum.C | TestEnum.D);

            Assert.False(actual);
        }

        [Fact]
        public void Should_Add()
        {
            var actual = FlagsEnum.Add(TestEnum.A | TestEnum.B, TestEnum.C);

            Assert.True(FlagsEnum.Has(actual, TestEnum.A));
            Assert.True(FlagsEnum.Has(actual, TestEnum.B));
            Assert.True(FlagsEnum.Has(actual, TestEnum.C));
        }

        [Fact]
        public void Should_Add_Idempotent()
        {
            var actual = FlagsEnum.Add(TestEnum.A | TestEnum.B, TestEnum.B);

            Assert.True(FlagsEnum.Has(actual, TestEnum.A));
            Assert.True(FlagsEnum.Has(actual, TestEnum.B));
        }

        [Fact]
        public void Should_Remove()
        {
            var actual = FlagsEnum.Remove(TestEnum.A | TestEnum.B, TestEnum.B);

            Assert.True(FlagsEnum.Has(actual, TestEnum.A));
            Assert.False(FlagsEnum.Has(actual, TestEnum.B));
        }

        [Fact]
        public void Should_Remove_Idempotent()
        {
            var actual = FlagsEnum.Remove(TestEnum.A, TestEnum.B);

            Assert.True(FlagsEnum.Has(actual, TestEnum.A));
            Assert.False(FlagsEnum.Has(actual, TestEnum.B));
        }

        [Fact]
        public void Should_Check_If_Zero_Has_No_Value()
        {
            var zero = FlagsEnum.None<TestEnum>();

            Assert.False(FlagsEnum.Has(zero, TestEnum.A));
            Assert.False(FlagsEnum.Has(zero, TestEnum.B));
            Assert.False(FlagsEnum.Has(zero, TestEnum.C));
            Assert.False(FlagsEnum.Has(zero, TestEnum.D));
        }

        [Fact]
        public void Should_Check_If_All_Has_All_Value()
        {
            var all = FlagsEnum.All<TestEnum>();

            Assert.True(FlagsEnum.Has(all, TestEnum.A));
            Assert.True(FlagsEnum.Has(all, TestEnum.B));
            Assert.True(FlagsEnum.Has(all, TestEnum.C));
            Assert.True(FlagsEnum.Has(all, TestEnum.D));
        }

        [Fact]
        public void Should_Expand()
        {
            var value = TestEnum.A | TestEnum.B;

            var result = FlagsEnum.Expand(value).ToArray();

            Assert.True(result.Length == 2);
            Assert.True(FlagsEnum.Has(result[0], TestEnum.A));
            Assert.True(FlagsEnum.Has(result[1], TestEnum.B));
        }

        [Fact]
        public void Should_Combine()
        {
            var value = FlagsEnum.Combine(TestEnum.A, TestEnum.B);

            Assert.True(FlagsEnum.Has(value, TestEnum.A));
            Assert.True(FlagsEnum.Has(value, TestEnum.B));
            Assert.False(FlagsEnum.Has(value, TestEnum.C));
            Assert.False(FlagsEnum.Has(value, TestEnum.D));
        }

        private enum TestEnum
        {
            A = 1,
            B = 1 << 1,
            C = 1 << 2,
            D = 1 << 3
        }

        private enum SmallEnum : byte
        {
            A = 1,
            B = 1 << 1,
            C = 1 << 2,
            D = 1 << 3
        }
    }
}
