namespace Examples;

public class DisposableExample : IDisposable
{
    private bool disposed;

    private object? largeObject;
    private IDisposable? disposable;

    public void Dispose()
    {
        if (disposed) return;

        disposed = true;

        // dispose inner values and nullify large objects
        largeObject = null;
        disposable?.Dispose();
    }
}
