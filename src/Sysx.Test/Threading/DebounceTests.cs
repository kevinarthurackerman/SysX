namespace Sysx.Test.Threading;
using Assert = Xunit.Assert;

public class DebounceTests
{
    [Fact]
    public void Should_Debounce()
    {
        using var testTimeMachine = new TestTimeMachine();
        var debounce = new Debounce(() => testTimeMachine.Now, delay => testTimeMachine.CreateDelay(delay));

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
        var debounce = new Debounce(() => testTimeMachine.Now, delay => testTimeMachine.CreateDelay(delay));

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
        var debounce = new Debounce(() => testTimeMachine.Now, delay => testTimeMachine.CreateDelay(delay));

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
        var executeAction = () => { executedCount++; };

        using var testTimeMachine = new TestTimeMachine();
        var debounce = new Debounce(
            () => testTimeMachine.Now,
            delay => testTimeMachine.CreateDelay(delay),
            executeAction,
            TimeSpan.FromMilliseconds(10));

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
