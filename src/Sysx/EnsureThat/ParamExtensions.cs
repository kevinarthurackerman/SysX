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

    private static Exception ThrowException<T>(Param<T> param, string defaultMessage, DefaultExceptionFactory defaultExceptionFactory)
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
