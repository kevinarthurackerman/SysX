namespace Sysx.Threading;

/// <summary>
/// Creates a debounce action that will execute after the invoke method has been
/// called and the timeout period has passed without calling the invoke method again.
/// Each call to the invoke method resets the timeout and may change the execute action
/// or the timeout duration.
/// </summary>
public class Debounce
{
    private static readonly DebounceOptions defaultOptions = DebounceOptions.Default;

    private readonly object @lock = new();
    private readonly DebounceOptions options;
    private Action? nextExecute;
    private DateTime nextInvoke = DateTime.MinValue;
    private Task? delayTask;

    public Debounce() : this(defaultOptions) { }

    public Debounce(in DebounceOptions options)
    {
        options.Validate();

        var createDelayInner = options.CreateDelay;

        this.options = new DebounceOptions
        {
            GetNow = options.GetNow,
            CreateDelay = delay =>
            {
                var delayTask = createDelayInner(delay);
                EnsureArg.IsNotNull(delayTask, nameof(delayTask));
                return delayTask;
            },
            DefaultExecute = options.DefaultExecute,
            DefaultDelay = options.DefaultDelay
        };
    }

    /// <inheritdoc cref="Invoke(Action?, TimeSpan?)" />
    public void Invoke() => Invoke(options.DefaultExecute, options.DefaultDelay);

    /// <inheritdoc cref="Invoke(Action?, TimeSpan?)" />
    public void Invoke(TimeSpan delay) => Invoke(options.DefaultExecute, delay);

    /// <inheritdoc cref="Invoke(Action?, TimeSpan?)" />
    public void Invoke(Action execute) => Invoke(execute, options.DefaultDelay);

    /// <summary>
    /// Invokes the Debounce, beginning or resetting the timeout until the execute action will be called.
    /// Execute and delay must either be provided at the call site or in the constructor. Providing
    /// execute and delay at the call site will override the values from the constructor.
    /// </summary>
    public void Invoke(Action? execute, TimeSpan? delay)
    {
        execute ??= options.DefaultExecute ?? throw new InvalidOperationException($"Must provide {nameof(execute)} at the {nameof(Invoke)} call site or {nameof(options.DefaultExecute)} when constructing {nameof(Debounce)}.");
        delay ??= options.DefaultDelay ?? throw new InvalidOperationException($"Must provide {nameof(delay)} at the {nameof(Invoke)} call site or {nameof(options.DefaultDelay)} when constructing {nameof(Debounce)}.");

        lock (@lock)
        {
            nextExecute = execute;
            var newNextInvoke = options.GetNow().Add(delay.Value);

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
                    await options.CreateDelay(delay).ConfigureAwait(false);

                    lock (@lock)
                    {
                        var now = options.GetNow();
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

    public record struct DebounceOptions
    {
        public static DebounceOptions Default => new()
        {
            GetNow = () => DateTime.UtcNow,
            CreateDelay = delay => Task.Delay(delay),
            DefaultExecute = null,
            DefaultDelay = null
        };

        public Func<DateTime> GetNow { get; set; }
        public Func<TimeSpan,Task> CreateDelay { get; set; }
        public Action? DefaultExecute { get; set; }
        public TimeSpan? DefaultDelay { get; set; }

        public void Validate()
        {
            EnsureArg.IsNotNull(GetNow, nameof(GetNow));
            EnsureArg.IsNotNull(CreateDelay, nameof(CreateDelay));
        }
    }
}