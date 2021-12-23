namespace Sysx.Test.Threading;
using Assert = Xunit.Assert;

public class DebounceTests
{
    [Fact]
    public void Should_Debounce()
    {
        using var testTimeMachine = new TestTimeMachine();
        var options = Debounce.DebounceOptions.Default with { GetNow = () => testTimeMachine.Now, CreateDelay = delay => testTimeMachine.CreateDelay(delay) };
        var debounce = new Debounce(in options);

        var executedCount = 0;
        var executeAction = () => { executedCount++; };

        debounce.Invoke(executeAction, TimeSpan.FromMilliseconds(10));

        Assert.Equal(0, executedCount);

        testTimeMachine.IncrementNow(TimeSpan.FromMilliseconds(1));

        Assert.Equal(0, executedCount);

        testTimeMachine.IncrementNow(TimeSpan.FromMilliseconds(100));

        Thread.Sleep(1);

        testTimeMachine.IncrementNow(TimeSpan.FromMilliseconds(100));

        Assert.Equal(1, executedCount);
    }

    [Fact]
    public void Should_Debounce_Once()
    {
        using var testTimeMachine = new TestTimeMachine();
        var options = Debounce.DebounceOptions.Default with { GetNow = () => testTimeMachine.Now, CreateDelay = delay => testTimeMachine.CreateDelay(delay) };
        var debounce = new Debounce(in options);

        var executedCount = 0;
        var executeAction = () => { executedCount++; };

        for (var i = 0; i < 4; i++)
        {
            debounce.Invoke(executeAction, TimeSpan.FromMilliseconds(10));

            testTimeMachine.IncrementNow(TimeSpan.FromMilliseconds(5));
        }

        Assert.Equal(0, executedCount);

        testTimeMachine.IncrementNow(TimeSpan.FromMilliseconds(100));

        Assert.Equal(1, executedCount);
    }

    [Fact]
    public void Should_Debounce_Again()
    {
        using var testTimeMachine = new TestTimeMachine();
        var options = Debounce.DebounceOptions.Default with { GetNow = () => testTimeMachine.Now, CreateDelay = delay => testTimeMachine.CreateDelay(delay) };
        var debounce = new Debounce(in options);

        var executedCount = 0;
        var executeAction = () => { executedCount++; };

        debounce.Invoke(executeAction, TimeSpan.FromMilliseconds(10));

        Assert.Equal(0, executedCount);

        testTimeMachine.IncrementNow(TimeSpan.FromMilliseconds(100));

        Assert.Equal(1, executedCount);

        debounce.Invoke(executeAction, TimeSpan.FromMilliseconds(10));

        Assert.Equal(1, executedCount);

        testTimeMachine.IncrementNow(TimeSpan.FromMilliseconds(100));

        Assert.Equal(2, executedCount);
    }

    [Fact]
    public void Should_Debounce_With_Constructor_Arguments()
    {
        var executedCount = 0;

        using var testTimeMachine = new TestTimeMachine();
        var options = Debounce.DebounceOptions.Default with 
        { 
            GetNow = () => testTimeMachine.Now, 
            CreateDelay = delay => testTimeMachine.CreateDelay(delay),
            DefaultExecute = () => { executedCount++; },
            DefaultDelay = TimeSpan.FromMilliseconds(10)
        };
        var debounce = new Debounce(in options);

        debounce.Invoke();

        Assert.Equal(0, executedCount);

        testTimeMachine.IncrementNow(TimeSpan.FromMilliseconds(1));

        Assert.Equal(0, executedCount);

        testTimeMachine.IncrementNow(TimeSpan.FromMilliseconds(100));

        Thread.Sleep(1);

        testTimeMachine.IncrementNow(TimeSpan.FromMilliseconds(100));

        Assert.Equal(1, executedCount);
    }
}
