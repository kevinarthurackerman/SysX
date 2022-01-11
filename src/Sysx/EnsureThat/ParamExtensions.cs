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

    public static void DoesNotContainNull<TEnumerable>(this Param<TEnumerable> param)
        where TEnumerable : IEnumerable
    {
        foreach(var value in param.Value)
        {
            if (value == null)
            {
                throw ThrowException(
                    param,
                    $"Value '{param.Name}' must not contain null values.",
                    message => new ArgumentNullException(param.Name, message));
            }
        }
    }

    public static void DoesNotContain<T, TEnumerable>(this Param<TEnumerable> param, T value)
        where TEnumerable : IEnumerable
    {
        if (value == null)
        {
            foreach (var val in param.Value)
                if (val == null) Throw();
        }
        else
        {
            foreach (var val in param.Value)
                if (val is T tVal && tVal.Equals(value)) Throw();
        }

        void Throw()
        {
            var commaSeparatedValues = string.Join(", ", param.Value);

            throw ThrowException(
                param,
                $"Value '{param.Name}' '[{commaSeparatedValues}]' must not contain a value of {value}.",
                message => new ArgumentException(param.Name, message));
        }
    }

    public static void DoesNotContain(this StringParam param, char value)
    {
        if (param.Value.All(x => x != value)) return;

        throw ThrowException(
            param,
            $"Value '{param.Name}' '{param.Value}' must not contain char {value}.",
            message => new ArgumentException(param.Name, message));
    }

    public static void IsNotContainedIn<T, TEnumerable>(this Param<T> param, TEnumerable value)
        where TEnumerable : IEnumerable<T>
    {
        if (param.Value == null)
        {
            foreach (var val in value)
                if (val == null) Throw();
        }
        else
        {
            foreach (var val in value)
                if (val is T tVal && tVal.Equals(param.Value)) Throw();
        }

        void Throw()
        {
            var commaSeparatedValues = string.Join(", ", value);

            throw ThrowException(
                param,
                $"Value '{param.Name}' '{param.Value}' must not be contained in '[{commaSeparatedValues}]'.",
                message => new ArgumentException(param.Name, message));
        }
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
