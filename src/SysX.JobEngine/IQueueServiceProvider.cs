namespace SysX.JobEngine;

/// <summary>
/// Used to access services provided by the queue.
/// </summary>
public interface IQueueServiceProvider : IServiceProvider
{
	internal void SetServiceProvider(IServiceProvider serviceProvider);
}
