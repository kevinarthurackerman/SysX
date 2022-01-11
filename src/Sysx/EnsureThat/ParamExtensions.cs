namespace Sysx.EnsureThat;

public static class ParamExtensions
{
    public static void IsNotDisposed<T>(this Param<T> param, bool isDisposed) where T : IDisposable
    {
        EnsureArg.HasValue(param.Value, nameof(param));

        if (!isDisposed) return;

        throw ThrowException(
            param,
            $"Disposed object {param.Name} can no longer be used.",
            message => new ObjectDisposedException(param.Value.GetType().FullName, message));
    }

    public static void DoesNotContainNull<T>(this Param<IEnumerable<T>> param)
    {
        if (param.Value.All(x => x != null)) return;

        throw ThrowException(
            param,
            $"Value '{param.Name}' must not contain null values.",
            message => new ArgumentNullException(param.Name, message));
    }

    public static void DoesNotContain<T>(this Param<IEnumerable<T>> param, T value)
    {
        if (value == null)
            if (param.Value.All(x => x != null)) return;
        else if (param.Value.Where(x => x != null).All(x => !x!.Equals(value))) return;

        var commaSeparatedValues = string.Join(", ", param.Value);

        throw ThrowException(
            param,
            $"Value '{param.Name}' '[{commaSeparatedValues}]' must not contain a value of {value}.",
            message => new ArgumentException(param.Name, message));
    }

    public static void DoesNotContain(this StringParam param, char value)
    {
        if (param.Value.All(x => x != value)) return;

        throw ThrowException(
            param,
            $"Value '{param.Name}' '{param.Value}' must not contain char {value}.",
            message => new ArgumentException(param.Name, message));
    }

    public static void IsNotContainedIn<T>(this Param<T> param, IEnumerable<T> value)
    {
        if (param.Value == null)
            if (value.All(x => x != null)) return;
            else if (value.Where(x => x != null).All(x => !x!.Equals(param.Value))) return;

        var commaSeparatedValues = string.Join(", ", value);

        throw ThrowException(
            param,
            $"Value '{param.Name}' '{param.Value}' must not be contained in '[{commaSeparatedValues}]'.",
            message => new ArgumentException(param.Name, message));
    }

    private static Exception ThrowException<T>(Param<T> param, string defaultMessage, DefaultExceptionFactory defaultExceptionFactory)
    {
        var options = param.OptsFn?.Invoke(new EnsureOptions()) ?? new EnsureOptions();

        var message = options.CustomMessage ?? defaultMessage;

        var exception = options.CustomExceptionFactory?.Invoke(message, param.Name)
            ?? options.CustomException
            ?? defaultExceptionFactory.Invoke(message);

        return exception;
    }

    private static Exception ThrowException(StringParam param, string defaultMessage, DefaultExceptionFactory defaultExceptionFactory)
    {
        var options = param.OptsFn?.Invoke(new EnsureOptions()) ?? new EnsureOptions();

        var message = options.CustomMessage ?? defaultMessage;

        var exception = options.CustomExceptionFactory?.Invoke(message, param.Name)
            ?? options.CustomException
            ?? defaultExceptionFactory.Invoke(message);

        return exception;
    }

    private delegate Exception DefaultExceptionFactory(string message);
}
