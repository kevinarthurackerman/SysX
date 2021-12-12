namespace Sysx.Threading;

/// <summary>
/// Creates a debounce action that will execute after the invoke method has been
/// called and the timeout period has passed without calling the invoke method again.
/// Each call to the invoke method resets the timeout and may change the execute action
/// or the timeout duration.
/// </summary>
public class Debounce
{
    private readonly object @lock = new();
    private readonly Func<DateTime> getNow;
    private readonly Func<TimeSpan, Task> createDelay;
    private readonly Action? defaultExecute;
    private readonly TimeSpan? defaultDelay;
    private Action? nextExecute;
    private DateTime nextInvoke = DateTime.MinValue;
    private Task? delayTask;

    public Debounce(Func<DateTime> getNow, Func<TimeSpan, Task> createDelay) :
        this(getNow, createDelay, null, null) { }

    public Debounce(Func<DateTime> getNow, Func<TimeSpan, Task> createDelay, TimeSpan defaultDelay) :
        this(getNow, createDelay, null, defaultDelay) { }

    public Debounce(Func<DateTime> getNow, Func<TimeSpan, Task> createDelay, Action defaultExecute) :
        this(getNow, createDelay, defaultExecute, null) { }

    public Debounce(Func<DateTime> getNow, Func<TimeSpan,Task> createDelay, Action? defaultExecute, TimeSpan? defaultDelay)
    {
        this.getNow = getNow;
        this.createDelay = createDelay;
        this.defaultExecute = defaultExecute;
        this.defaultDelay = defaultDelay;
    }

    /// <inheritdoc cref="Invoke(Action?, TimeSpan?)" />
    public void Invoke() => Invoke(defaultExecute, defaultDelay);

    /// <inheritdoc cref="Invoke(Action?, TimeSpan?)" />
    public void Invoke(TimeSpan delay) => Invoke(defaultExecute, delay);

    /// <inheritdoc cref="Invoke(Action?, TimeSpan?)" />
    public void Invoke(Action execute) => Invoke(execute, defaultDelay);

    /// <summary>
    /// Invokes the Debounce, beginning or resetting the timeout until the execute action will be called.
    /// Execute and delay must either be provided at the call site or in the constructor. Providing
    /// execute and delay at the call site will override the values from the constructor.
    /// </summary>
    public void Invoke(Action? execute, TimeSpan? delay)
    {
        execute ??= defaultExecute ?? throw new InvalidOperationException($"Must provide {nameof(execute)} at the {nameof(Invoke)} call site or {nameof(defaultExecute)} when constructing {nameof(Debounce)}.");
        delay ??= defaultDelay ?? throw new InvalidOperationException($"Must provide {nameof(delay)} at the {nameof(Invoke)} call site or {nameof(defaultDelay)} when constructing {nameof(Debounce)}.");

        lock (@lock)
        {
            nextExecute = execute;
            var newNextInvoke = getNow().Add(delay.Value);

            if (newNextInvoke > nextInvoke)
            {
                nextInvoke = newNextInvoke;

                if (delayTask == null)
                {
                    ScheduleExecute(delay.Value);
                }
            }
        }
    }

    private void ScheduleExecute(TimeSpan delay)
    {
        delayTask = Task.Run(async () =>
            {
                Action? doExecute = null;

                do
                {
                    await createDelay(delay).ConfigureAwait(false);

                    lock (@lock)
                    {
                        var now = getNow();
                        if (nextInvoke <= now)
                        {
                            doExecute = nextExecute;
                            delayTask = null;
                        }
                        else
                        {
                            delay = nextInvoke.Subtract(now);
                        }
                    }
                }
                while (doExecute == null);

                // handle exceptions?
                doExecute.Invoke();
            });
    }
}