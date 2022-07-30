namespace Examples;

public class DisposableExample : IDisposable
{
	private bool disposed;

	private IDisposable? disposable;

	private object? largeObject;

	public DisposableExample(IDisposable? disposable, object? largeObject)
	{
		this.disposable = disposable;
		this.largeObject = largeObject;
	}

	public void Dispose()
	{
		if (disposed) return;

		disposed = true;

		// dispose inner values and nullify large objects
		disposable?.Dispose();
		largeObject = null;
	}
}
