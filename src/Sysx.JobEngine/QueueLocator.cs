namespace Sysx.JobEngine;

public interface IQueueLocator
{
    public TQueue Get<TQueue>()
        where TQueue : IQueue;

    public TQueue Get<TQueue>(string name)
        where TQueue : IQueue;

    internal void RegisterQueue<TQueue>(TQueue queue)
        where TQueue : IQueue;

    internal void RegisterQueue<TQueue>(TQueue queue, string name)
        where TQueue : IQueue;

    internal void DeregisterQueue<TQueue>()
        where TQueue : IQueue;

    internal void DeregisterQueue<TQueue>(string name)
        where TQueue : IQueue;
}

public class QueueLocator : IQueueLocator
{
    private readonly Dictionary<QueueKey, IQueue> queues = new();

    public TQueue Get<TQueue>()
        where TQueue : IQueue
        => (TQueue)queues[new QueueKey(typeof(TQueue), "Default")];

    public TQueue Get<TQueue>(string name)
        where TQueue : IQueue
        => (TQueue)queues[new QueueKey(typeof(TQueue), name)];

    void IQueueLocator.RegisterQueue<TQueue>(TQueue queue)
        => queues.Add(new QueueKey(typeof(TQueue), "Default"), queue);

    void IQueueLocator.RegisterQueue<TQueue>(TQueue queue, string name)
        => queues.Add(new QueueKey(typeof(TQueue), name), queue);

    void IQueueLocator.DeregisterQueue<TQueue>()
        => queues.Remove(new QueueKey(typeof(TQueue), "Default"));

    void IQueueLocator.DeregisterQueue<TQueue>(string name)
        => queues.Remove(new QueueKey(typeof(TQueue), name));

    private readonly record struct QueueKey(Type Type, string Name);
}
