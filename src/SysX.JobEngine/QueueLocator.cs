namespace SysX.JobEngine;

// todo: Something needs to be done to make this thread safe since it can be accessed by multiple jobs on different threads at the same time.
// A possible solution is to present the same Queues for the duration of a Transaction and prevent those Queues from being disposed until it closes.

/// <summary>
/// A service used to get queues.
/// </summary>
public class QueueLocator : IQueueLocator
{
	private readonly Dictionary<QueueKey, IQueue> queues = new();

	public const string DefaultQueueName = "Default";

	/// <summary>
	/// Gets a queue.
	/// </summary>
	public IQueue Get(Type queueType, string name)
	{
		EnsureArg.IsNotNull(queueType, nameof(queueType));
		EnsureArg.IsTrue(
			typeof(IQueue).IsAssignableFrom(queueType),
			optsFn: x => x.WithMessage($"Type {nameof(queueType)} must be assignable to {typeof(IQueue)}"));
		EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

		return queues[new QueueKey(queueType, name)];
	}

	/// <summary>
	/// Gets all queues.
	/// </summary>
	public IEnumerable<IQueue> GetAll() => queues.Values.ToArray();

	void IQueueLocator.Register(IQueue queue, string name)
	{
		EnsureArg.IsNotNull(queue, nameof(queue));
		EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

		var key = new QueueKey(queue.GetType(), name);

		EnsureArg.IsFalse(queues.ContainsKey(key), optsFn: x => x.WithMessage("A queue with this type and name already exists."));

		queues[new QueueKey(queue.GetType(), name)] = queue;
	}

	void IQueueLocator.Deregister(Type queueType, string name)
	{
		EnsureArg.IsNotNull(queueType, nameof(queueType));
		EnsureArg.IsTrue(
			typeof(IQueue).IsAssignableFrom(queueType),
			optsFn: x => x.WithMessage($"Type {nameof(queueType)} must be assignable to {typeof(IQueue)}"));
		EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

		queues.Remove(new QueueKey(queueType, name));
	}

	private readonly record struct QueueKey(Type Type, string Name);
}
