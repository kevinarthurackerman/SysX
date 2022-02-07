# Sysx.Reflection.DuckTyper

Allows wrapping types with an interface at runtime so that they can be accessed via duck-typing or as a "friend class".

**Duck-typing** is the ability to access the members of a class that match a member signature, similar to accessing a class through an interface definition.

**Friend Class**es are classes that have access to the private members of another class.

The following demonstrates using duck typing to access the members of a class:

```csharp
public class Test
{
    [Xunit.Fact]
    public void Should_Wrap_And_Set_Value()
    {
        var value = new Duck();
        var wrapper = Sysx.Reflection.DuckTyper.Wrap<IDuck>(value);

        wrapper.Quack = "Quack";

        Xunit.Assert.Equal("Quack", value.Quack);
        Xunit.Assert.Equal("Quack", wrapper.Quack);
    }

    public interface IDuck
    {
        public string? Quack { get; set; }
    }

    public class Duck
    {
        public string? Quack { get; set; }
    }
}
```

Similarly we can access the class as a friended class as follows:

```csharp
public class Test
{
    [Xunit.Fact]
    public void Should_Wrap_And_Set_Value()
    {
        var value = new FriendlyDuck();
        var wrapper = Sysx.Reflection.DuckTyper.Wrap<IFriendedDuck>(value, includePrivateMembers: true);

        wrapper.quack = "Quack";

        Xunit.Assert.Equal("Quack", wrapper.quack);
    }

    public interface IFriendedDuck
    {
        public string? quack { get; set; }
    }

    public class FriendlyDuck
    {
        private string? quack;
    }
}
```

And finally, if we are not sure if what we have is duck-like we can try to wrap it:

```csharp
public class Test
{
    [Xunit.Fact]
    public void Should_Wrap_And_Set_Value()
    {
        var possiblyDucks = new object[]
        {
            new Duck(),
            new NotDuck()
        };

        foreach (var potentialDuck in possiblyDucks)
        {
            if (Sysx.Reflection.DuckTyper.TryWrap<IDuck>(potentialDuck, out var duck))
            {
                duck.Quack = "Quack";

                Xunit.Assert.Equal("Quack", duck.Quack);
            }
        }
    }

    public interface IDuck
    {
        public string? Quack { get; set; }
    }

    public class Duck
    {
        public string? Quack { get; set; }
    }

    public class NotDuck { }
}
```

See [here](https://github.com/kevinarthurackerman/Sysx/tree/main/src/Test_Sysx/Reflection/DuckTyper) for more tests and examples of use.

See [here](https://github.com/kevinarthurackerman/Sysx/blob/main/src/Sysx/Reflection/DuckTyper.cs) for the implementation of the DuckTyper static class.
