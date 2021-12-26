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
}
