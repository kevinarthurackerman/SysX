namespace Sysx.Threading;

/// <summary>
/// Creates a debounce action that will execute after the <see cref="Invoke"/> method has been
/// called and the timeout period has passed without calling <see cref="Invoke"/> again.
/// Each call to <see cref="Invoke"/> method resets the timeout and may change the execute action
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

    /// <summary>
    /// Initializes a <see cref="Debounce"/> with the default options.
    /// </summary>
    public Debounce() : this(defaultOptions) { }

    /// <summary>
    /// Initializes a <see cref="Debounce"/> with the given options.
    /// </summary>
    /// <param name="options"></param>
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
    /// Invokes the <see cref="Debounce"/>, beginning or resetting the timeout until the execute action will be called.
    /// Parameters <paramref name="execute"/> and <paramref name="delay"/> must either be provided at the call site or in the constructor. Providing
    /// <paramref name="execute"/> and <paramref name="delay"/> at the call site will override the values from the constructor.
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

    /// <summary>
    /// Options for initializing a <see cref="Debouce"/>.
    /// </summary>
    public record struct DebounceOptions
    {
        /// <summary>
        /// The default options for initializing a <see cref="Debouce"/>.
        /// </summary>
        public static DebounceOptions Default => new()
        {
            GetNow = () => DateTime.UtcNow,
            CreateDelay = delay => Task.Delay(delay),
            DefaultExecute = null,
            DefaultDelay = null
        };

        /// <summary>
        /// Gets the current "now" time.
        /// </summary>
        public Func<DateTime> GetNow { get; set; }

        /// <summary>
        /// <see cref="Func{T, TResult}"/> that creates a delay <see cref="Task"/> given a <see cref="TimeSpan"/>.
        /// </summary>
        public Func<TimeSpan,Task> CreateDelay { get; set; }

        /// <summary>
        /// The default <see cref="Action{T1}"/> to execute after the delay.
        /// </summary>
        public Action? DefaultExecute { get; set; }

        /// <summary>
        /// The default amount of time to wait before executing.
        /// </summary>
        public TimeSpan? DefaultDelay { get; set; }

        /// <summary>
        /// Validates this options configuration.
        /// </summary>
        public void Validate()
        {
            EnsureArg.IsNotNull(GetNow, nameof(GetNow));
            EnsureArg.IsNotNull(CreateDelay, nameof(CreateDelay));
        }
    }
}