namespace Sysx.EnsureThat
{
    public static class ParamExtensions
    {
        public static void IsNotDisposed<T>(this Param<T> param, bool isDisposed) where T : IDisposable
        {
            EnsureArg.HasValue(param.Value, nameof(param));

            var options = param.OptsFn?.Invoke(new EnsureOptions()) ?? new EnsureOptions();

            if (!isDisposed) return;

            var message = options.CustomMessage ?? $"Disposed object {param.Name} can no longer be used.";

            var exception = options.CustomExceptionFactory?.Invoke(message, param.Name)
                ?? options.CustomException
                ?? new ObjectDisposedException(param.Value.GetType().FullName, "Disposed object can no longer be used.");

            throw exception;
        }
    }
}
