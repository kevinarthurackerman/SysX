namespace Test_Sysx.EnsureThat;
using Assert = Xunit.Assert;

public class ParamExtensions
{
    [Fact]
    public void Should_Throw_When_Disposed()
    {
        var testObj = new Disposable();
        testObj.Dispose();

        Assert.Throws<ObjectDisposedException>(() => testObj.DoThing());
    }

    [Fact]
    public void Should_Not_Throw_When_Not_Disposed()
    {
        var testObj = new Disposable();

        var exception = Record.Exception(() => testObj.DoThing());

        Assert.Null(exception);
    }

    [Fact]
    public void Should_Throw_When_Contains_Null()
    {
        var testObj = new int?[] { 1, 2, null };

        Assert.Throws<ArgumentNullException>(() => Ensure.That(testObj).DoesNotContainNull());
    }

    [Fact]
    public void Should_Not_Throw_When_Not_Contains_Null()
    {
        var testObj = new int?[] { 1, 2, 3 };

        var exception = Record.Exception(() => Ensure.That(testObj).DoesNotContainNull());

        Assert.Null(exception);
    }

    [Fact]
    public void Should_Throw_When_Contains_Value()
    {
        var testObj = new int[] { 1, 2, 3 };
        
        Assert.Throws<ArgumentException>(() => Ensure.That(testObj).DoesNotContain(3));
    }

    [Fact]
    public void Should_Not_Throw_When_Not_Contains_Value()
    {
        var testObj = new int[] { 1, 2, 4 };

        var exception = Record.Exception(() => Ensure.That(testObj).DoesNotContain(3));

        Assert.Null(exception);
    }

    [Fact]
    public void Should_Throw_When_Contains_Char()
    {
        var testObj = "test";

        Assert.Throws<ArgumentException>(() => Ensure.That(testObj).DoesNotContain('t'));
    }

    [Fact]
    public void Should_Not_Throw_When_Not_Contains_Char()
    {
        var testObj = "es";

        var exception = Record.Exception(() => Ensure.That(testObj).DoesNotContain('t'));

        Assert.Null(exception);
    }

    [Fact]
    public void Should_Throw_When_Value_Is_Contained_In()
    {
        var testObj = new int[] { 1, 2, 3 };

        Assert.Throws<ArgumentException>(() => Ensure.That(3).IsNotContainedIn(testObj));
    }

    [Fact]
    public void Should_Not_Throw_When_Value_Is_Not_Contained_In()
    {
        var testObj = new int[] { 1, 2, 4 };

        var exception = Record.Exception(() => Ensure.That(3).IsNotContainedIn(testObj));

        Assert.Null(exception);
    }

    public class Disposable : IDisposable
    {
        private bool disposed = false;

        public void DoThing()
        {
            Ensure.That(this).IsNotDisposed(disposed);
        }

        public void Dispose()
        {
            disposed = true;
        }
    }
}
