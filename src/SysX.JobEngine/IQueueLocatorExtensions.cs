namespace SysX.JobEngine;

public static class IQueueLocatorExtensions
{
    /// <inheritdoc cref="IQueueLocator.Get(Type, string)" />
    public static IQueue Get(this IQueueLocator queueLocator)
        => queueLocator.Get(typeof(IQueue), QueueLocator.DefaultQueueName);

    /// <inheritdoc cref="IQueueLocator.Get(Type, string)" />
    public static IQueue Get(this IQueueLocator queueLocator, string name)
        => queueLocator.Get(typeof(IQueue), name);

    /// <inheritdoc cref="IQueueLocator.Get(Type, string)" />
    public static TQueue Get<TQueue>(this IQueueLocator queueLocator)
        where TQueue : IQueue
        => (TQueue)queueLocator.Get(typeof(TQueue), QueueLocator.DefaultQueueName);

    /// <inheritdoc cref="IQueueLocator.Get(Type, string)" />
    public static TQueue Get<TQueue>(this IQueueLocator queueLocator, string name)
        where TQueue : IQueue
        => (TQueue)queueLocator.Get(typeof(TQueue), name);

    /// <inheritdoc cref="IQueueLocator.GetAll" />
    public static IEnumerable<TQueue> GetAll<TQueue>(this IQueueLocator queueLocator)
        where TQueue : IQueue
        => queueLocator.GetAll().OfType<TQueue>();
}
