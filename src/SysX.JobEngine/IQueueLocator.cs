namespace SysX.JobEngine;

/// <summary>
/// A service used to get queues.
/// </summary>
public interface IQueueLocator
{
	/// <summary>
	/// Gets a queue.
	/// </summary>
	public IQueue Get(Type queueType, string name);

	/// <summary>
	/// Gets all queues.
	/// </summary>
	public IEnumerable<IQueue> GetAll();

	internal void Register(IQueue queue, string name);

	internal void Deregister(Type queueType, string name);
}
