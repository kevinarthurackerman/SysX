namespace Sysx.EnsureThat
{
    public static class ParamExtensions
    {
        public static void IsNotDisposed<T>(this Param<T> param, bool isDisposed) where T : IDisposable
        {
            if (isDisposed) throw new ObjectDisposedException(param.Value.GetType().FullName, "Disposed object can no longer be used.");
        }
    }
}
