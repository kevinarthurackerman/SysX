using System;
using Sysx.Reflection;

namespace Sysx.Test.Reflection
{
    using Xunit;

    public class DuckTyperTests
    {
        [Fact]
        public void Should_Wrap_Field()
        {
            var value = new DuckLikeClass { quackField = "quack" };
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("quack", valueWrapper.quackField);

            valueWrapper.quackField = "quack!";

            Assert.Equal("quack!", value.quackField);
        }

        [Fact]
        public void Should_Wrap_Readonly_Field()
        {
            var value = new DuckLikeClass();
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("quack", valueWrapper.quackFieldReadonly);

            Assert.Throws<InvalidOperationException>(() => valueWrapper.quackFieldReadonly = "quack!");

            Assert.Equal("quack", valueWrapper.quackFieldReadonly);
        }

        [Fact]
        public void Should_Access_Inherited_Field()
        {
            var value = new DuckLikeClassExtension { quackField = "Quack" };
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("Quack", valueWrapper.quackField);

            valueWrapper.quackField = "Quack!";

            Assert.Equal("Quack!", value.quackField);
        }

        [Fact]
        public void Should_Wrap_Property()
        {
            var value = new DuckLikeClass { QuackProp = "Quack" };
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("Quack", valueWrapper.QuackProp);

            valueWrapper.QuackProp = "Quack!";

            Assert.Equal("Quack!", value.QuackProp);
        }

        [Fact]
        public void Should_Access_Inherited_Property()
        {
            var value = new DuckLikeClassExtension { QuackProp = "Quack" };
            var valueWrapper = DuckTyper.Wrap<DuckLikeClassExtension, IDuck>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("Quack", valueWrapper.QuackProp);

            valueWrapper.QuackProp = "Quack!";

            Assert.Equal("Quack!", value.QuackProp);
        }

        [Fact]
        public void Should_Access_Virtual_Property()
        {
            var value = new DuckLikeClass { VirtualQuackProp = "Quack" };
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("Quack", valueWrapper.VirtualQuackProp);

            valueWrapper.VirtualQuackProp = "Quack!";

            Assert.Equal("Quack!", value.VirtualQuackProp);
        }

        [Fact]
        public void Should_Wrap_Property_With_Private_Getter()
        {
            var value = new DuckLikeClass { QuackPropPrivateGet = "Quack" };
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("Quack", value.GetQuackPropPrivateGetValue());

            Assert.Throws<InvalidOperationException>(() => valueWrapper.QuackPropPrivateGet);

            valueWrapper.QuackPropPrivateGet = "Quack!";

            Assert.Equal("Quack!", value.GetQuackPropPrivateGetValue());
        }

        [Fact]
        public void Should_Wrap_Property_With_Private_Setter()
        {
            var value = new DuckLikeClass();
            value.SetQuackPropPrivateSetValue("Quack");

            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("Quack", value.QuackPropPrivateSet);

            Assert.Throws<InvalidOperationException>(() => valueWrapper.QuackPropPrivateSet = "test");

            Assert.Equal("Quack", value.QuackPropPrivateSet);

            value.SetQuackPropPrivateSetValue("Quack!");

            Assert.Equal("Quack!", value.QuackPropPrivateSet);
        }

        [Fact]
        public void Should_Wrap_Get_Method()
        {
            var value = new DuckLikeClass();
            value.SetQuackPropPrivateSetValue("quack");
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("quack", valueWrapper.QuackPropPrivateSet);
        }

        [Fact]
        public void Should_Wrap_Set_Method()
        {
            var value = new DuckLikeClass();
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);

            valueWrapper.QuackPropPrivateGet = "quack";

            Assert.Equal("quack", valueWrapper.GetQuackPropPrivateGetValue());
        }

        [Fact]
        public void Should_Wrap_GetSet_Method()
        {
            var value = new DuckLikeClass();
            value.QuackPropPrivate("quack");
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("quack", valueWrapper.QuackPropPrivate("quack!"));
            Assert.Equal("quack!", valueWrapper.QuackPropPrivate("quack"));
        }

        [Fact]
        public void Should_Wrap_Parameterless_Void_Method()
        {
            var value = new DuckLikeClass();
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            valueWrapper.Quack();

            Assert.NotNull(valueWrapper);
            Assert.Equal("Quack", value.QuackProp);
        }

        [Fact]
        public void Should_Wrap_Second_Type()
        {
            var value = new CowLikeClass { MooProp = "Moo" };
            var valueWrapper = DuckTyper.Wrap<CowLikeClass, ICow>(value);

            Assert.NotNull(valueWrapper);
            Assert.Equal("Moo", valueWrapper.MooProp);

            valueWrapper.MooProp = "Moo!";

            Assert.Equal("Moo!", value.MooProp);
        }

        [Fact]
        public void Should_Not_Wrap_To_Wrong_Type()
        {
            var value = new DuckLikeClass { QuackWithWrongType = "Quack" };
            var valueWrapper = DuckTyper.Wrap<DuckLikeClass, IDuck>(value);

            Assert.NotNull(valueWrapper);

            Assert.Throws<InvalidOperationException>(() => valueWrapper.QuackWithWrongType);

            Assert.Equal("Quack", value.QuackWithWrongType);
        }

        public interface IDuck
        {
            string? PropMissingFromDuckLikeClass { get; set; }
            string? quackField { get; set; }
            string? quackFieldReadonly { get; set; }
            string? QuackProp { get; set; }
            string? VirtualQuackProp { get; set; }
            public string? QuackPropPrivateSet { get; set; }
            public string? QuackPropPrivateGet { get; set; }
            public object? QuackWithWrongType { get; set; }
            void SetQuackPropPrivateSetValue(string? value);
            string? GetQuackPropPrivateGetValue();
            string? QuackPropPrivate(string? value);
            void Quack();
        }

        public class DuckLikeClass
        {
            private string? quackPrivate;
            public string? NonDucklikeProp { get; set; }
            public string? quackField;
            public readonly string? quackFieldReadonly = "quack";
            public string? QuackProp { get; set; }
            public virtual string? VirtualQuackProp { get; set; }
            public string? QuackPropPrivateSet { get; private set; }
            public string? QuackPropPrivateGet { private get; set; }
            public string? QuackWithWrongType { get; set; }

            public void SetQuackPropPrivateSetValue(string? value) => QuackPropPrivateSet = value;
            public string? GetQuackPropPrivateGetValue() => QuackPropPrivateGet;
            public string? QuackPropPrivate(string? value)
            {   
                var val = quackPrivate;
                quackPrivate = value;
                return val;
            }
            public void Quack() => QuackProp = "Quack";
        }

        public class DuckLikeClassExtension : DuckLikeClass { }

        public interface ICow
        {
            string? MooProp { get; set; }
        }

        public class CowLikeClass
        {
            public string? MooProp { get; set; }
        }
    }
}
