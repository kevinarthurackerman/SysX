namespace Sysx.JobEngine;

public static class IQueueLocatorExtensions
{
    public static IQueue Get(this IQueueLocator queueLocator)
        => queueLocator.Get(typeof(IQueue), QueueLocator.DefaultQueueName);

    public static IQueue Get(this IQueueLocator queueLocator, string name)
        => queueLocator.Get(typeof(IQueue), name);

    public static TQueue Get<TQueue>(this IQueueLocator queueLocator)
        where TQueue : IQueue
        => (TQueue)queueLocator.Get(typeof(TQueue), QueueLocator.DefaultQueueName);

    public static TQueue Get<TQueue>(this IQueueLocator queueLocator, string name)
        where TQueue : IQueue
        => (TQueue)queueLocator.Get(typeof(TQueue), name);

    public static IEnumerable<TQueue> GetAll<TQueue>(this IQueueLocator queueLocator)
        where TQueue : IQueue
        => queueLocator.GetAll().OfType<TQueue>();
}

public interface IQueueLocator
{
    public IQueue Get(Type queueType, string name);
    public IEnumerable<IQueue> GetAll();
    internal void Register(IQueue queue, string name);
    internal void Deregister(Type queueType, string name);
}

public class QueueLocator : IQueueLocator
{
    private readonly Dictionary<QueueKey, IQueue> queues = new();

    public const string DefaultQueueName = "Default";

    public IQueue Get(Type queueType, string name)
    {
        EnsureArg.IsNotNull(queueType, nameof(queueType));
        EnsureArg.IsTrue(
            typeof(IQueue).IsAssignableFrom(queueType),
            optsFn: x => x.WithMessage($"Type {nameof(queueType)} must be assignable to {typeof(IQueue)}"));
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

        return queues[new QueueKey(queueType, name)];
    }

    public IEnumerable<IQueue> GetAll() => queues.Values.ToArray();

    void IQueueLocator.Register(IQueue queue, string name)
    {
        EnsureArg.IsNotNull(queue, nameof(queue));
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

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
