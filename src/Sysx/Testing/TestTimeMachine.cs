namespace Sysx.Testing;

/// <summary>
/// Used for testing to mock out the passage of time.
/// </summary>
public class TestTimeMachine : IDisposable
{
    private static readonly DateTime defaultNowTime = new(2000, 1, 1);
    private static readonly TimeSpan defaultTimeToWaitForOtherWork = TimeSpan.FromMilliseconds(200);

    private readonly List<Delay> delays = new();

    private bool disposed;

    public DateTime Now { get; private set; }

    /// <summary>
    /// After processing each delay the system will wait this long for other code to execute
    /// before processing additional delays.
    /// </summary>
    public TimeSpan DefaultTimeToWaitForOtherWork { get; }

    public TestTimeMachine() : this(defaultNowTime, defaultTimeToWaitForOtherWork) { }

    public TestTimeMachine(TimeSpan defaultTimeToWaitForOtherWork) : this(defaultNowTime, defaultTimeToWaitForOtherWork) { }

    public TestTimeMachine(DateTime now) : this(now, defaultTimeToWaitForOtherWork) { }

    public TestTimeMachine(DateTime now, TimeSpan defaultTimeToWaitForOtherWork)
    {
        Now = now;
        DefaultTimeToWaitForOtherWork = defaultTimeToWaitForOtherWork;
    }

    /// <inheritdoc cref="CreateDelay(DateTime, TimeSpan)" />
    public Task CreateDelay(TimeSpan delay) => CreateDelay(Now.Add(delay), defaultTimeToWaitForOtherWork);

    /// <inheritdoc cref="CreateDelay(DateTime, TimeSpan)" />
    public Task CreateDelay(TimeSpan delay, TimeSpan timeToWaitForOtherWork) => CreateDelay(Now.Add(delay), timeToWaitForOtherWork);

    /// <inheritdoc cref="CreateDelay(DateTime, TimeSpan)" />
    public Task CreateDelay(DateTime completesAt) => CreateDelay(completesAt, defaultTimeToWaitForOtherWork);

    /// <summary>
    /// Creates a mock delay that will be completed when the time is incremented passed it's completion time.
    /// The system will wait at least timeToWaitForOtherWork amount of time before continuing.
    /// </summary>
    public Task CreateDelay(DateTime completesAt, TimeSpan timeToWaitForOtherWork)
    {
        Ensure.That(this).IsNotDisposed(disposed);
        EnsureArg.IsGte(completesAt, Now);

        var completionSource = new TaskCompletionSource<object?>();
        delays.Add(new Delay(completesAt, completionSource, timeToWaitForOtherWork));

        return completionSource.Task;
    }

    /// <inheritdoc cref="SetNow(DateTime)" />
    public void IncrementNow(TimeSpan time) =>
        SetNow(Now.Add(time));

    /// <summary>
    /// Moves time forward and executes any delays that have occurred in that time period.
    /// </summary>
    /// <param name="now"></param>
    public void SetNow(DateTime now)
    {
        Ensure.That(this).IsNotDisposed(disposed);
        EnsureArg.IsGte(now, Now);
        
        while (true)
        {
            // Waits one tick so that task continuations and awaits fire
            Task.Delay(1).Wait();

            var nextCompletedDelays = delays
                .GroupBy(x => x.CompletesAt)
                .OrderBy(x => x.Key)
                .Select(x => new 
                {
                    CompletesAt = x.Key,
                    Delays = x.ToArray()
                })
                .FirstOrDefault();

            if (nextCompletedDelays == null || nextCompletedDelays.CompletesAt > now) break;

            Now = nextCompletedDelays.CompletesAt;

            foreach(var delay in nextCompletedDelays.Delays)
            {
                delay.CompletionSource.SetResult(null);

                delays.Remove(delay);
            }

            var timeToWait = nextCompletedDelays.Delays.Max(x => x.TimeToWaitForOtherWork);

            // Wait for other work to finish
            Thread.Sleep(timeToWait);
        }

        Now = now;
    }

    public void Dispose()
    {
        if (disposed) return;

        disposed = true;

        foreach (var delay in delays)
            delay.CompletionSource.SetCanceled();
    }

    public override bool Equals(object? obj)
    {
        Ensure.That(this).IsNotDisposed(disposed);

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        Ensure.That(this).IsNotDisposed(disposed);

        return base.GetHashCode();
    }

    public override string? ToString()
    {
        Ensure.That(this).IsNotDisposed(disposed);

        return base.ToString();
    }

    public record struct Delay(DateTime CompletesAt, TaskCompletionSource<object?> CompletionSource, TimeSpan TimeToWaitForOtherWork);
}