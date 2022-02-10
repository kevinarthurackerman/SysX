namespace SysX.JobEngine;

/// <summary>
/// Creates and manages queues for running jobs.
/// </summary>
public class Engine : IDisposable
{
    private readonly Dictionary<QueueKey, QueueInfo> queues;
    private readonly IServiceCollection defaultQueueServices;
    private readonly IServiceProvider engineServices;
    private bool disposed;

    public Engine(in EngineOptions createEngineOptions)
    {
        EnsureArg.HasValue(createEngineOptions, nameof(createEngineOptions));

        createEngineOptions.Validate();

        queues = new();
        disposed = false;

        var engineServices = new ServiceCollection()
            .AddScoped<IEngineServiceProvider>(engineServices => new InternalServiceProvider(engineServices))
            .AddSingleton<IQueueLocator, QueueLocator>();

        createEngineOptions.ConfigureEngineServices(engineServices);

        this.engineServices = engineServices.BuildServiceProvider();

        var defaultQueueServices = new ServiceCollection()
            .AddSingleton(typeof(IOnJobExecuteEvent<,>), typeof(OnJobExecute_TransactionScope<,>))
            .AddSingleton(queueServices => queueServices
                .GetRequiredService<IEngineServiceProvider>()
                .GetRequiredService<IQueueLocator>());

        createEngineOptions.DefaultConfigureQueueServices(defaultQueueServices);

        this.defaultQueueServices = new ServiceCollection();
        foreach (var service in defaultQueueServices)
            this.defaultQueueServices.Add(service);
    }

    /// <inheritdoc cref="CreateQueue{TQueue}(in CreateQueueOptions)" />
    public IQueue CreateQueue(in CreateQueueOptions createQueueOptions)
        => CreateQueue<IQueue>(in createQueueOptions);

    /// <summary>
    /// Creates a queue.
    /// </summary>
    public TQueue CreateQueue<TQueue>(in CreateQueueOptions createQueueOptions)
        where TQueue : IQueue
    {
        EnsureArg.HasValue(createQueueOptions, nameof(createQueueOptions));
        Ensure.That(this).IsNotDisposed(disposed);

        createQueueOptions.Validate();

        var queueScope = engineServices.CreateScope();

        var queueServices = new ServiceCollection()
            .AddSingleton<IEngineServiceProvider>(_ => new InternalServiceProvider(queueScope.ServiceProvider))
            .AddScoped<IQueueServiceProvider>(queueServices => new InternalServiceProvider(queueServices))
            .AddSingleton<IQueueContext, QueueContext>();

        foreach (var service in defaultQueueServices)
            queueServices.Add(service);

        createQueueOptions.ConfigureQueueServices(queueServices);

        var queueServiceProvider = queueServices.BuildServiceProvider();

        // todo: It needs to be determined if the queue should resolve it's dependencies from the enginer service provider
        // or from the queue's own service provider.
        var queue = queueScope.ServiceProvider.Activate<TQueue>(new InternalServiceProvider(queueServiceProvider));

        queueServiceProvider.GetRequiredService<IQueueContext>().SetCurrent(queue);

        queues.Add(new QueueKey(typeof(TQueue), createQueueOptions.Name), new QueueInfo(queue, queueScope));

        engineServices.GetRequiredService<IQueueLocator>()
            .Register(queue, createQueueOptions.Name);

        return queue;
    }

    /// <inheritdoc cref="RemoveQueueInner(Type, string, bool)" />
    public void RemoveQueue()
        => RemoveQueueInner(typeof(IQueue), QueueLocator.DefaultQueueName, checkDisposed: true);

    /// <inheritdoc cref="RemoveQueueInner(Type, string, bool)" />
    public void RemoveQueue(string name)
        => RemoveQueueInner(typeof(IQueue), name, checkDisposed: true);

    /// <inheritdoc cref="RemoveQueueInner(Type, string, bool)" />
    public void RemoveQueue<TQueue>()
        where TQueue : IQueue
        => RemoveQueueInner(typeof(TQueue), QueueLocator.DefaultQueueName, checkDisposed: true);

    /// <inheritdoc cref="RemoveQueueInner(Type, string, bool)" />
    public void RemoveQueue<TQueue>(string name)
        where TQueue : IQueue
        => RemoveQueueInner(typeof(TQueue), name, checkDisposed: true);

    /// <summary>
    /// Removes a queue, waits for pending jobs to complete, and disposes of it's services.
    /// </summary>
    private void RemoveQueueInner(Type queueType, string name, bool checkDisposed)
    {
        if (checkDisposed) Ensure.That(this).IsNotDisposed(disposed);
        EnsureArg.IsNotNull(queueType, nameof(queueType));
        EnsureArg.IsTrue(
            typeof(IQueue).IsAssignableFrom(queueType),
            optsFn: x => x.WithMessage($"Type {nameof(queueType)} must be assignable to {typeof(IQueue)}"));
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

        var key = new QueueKey(queueType, name);

        var queue = queues[key];

        queues.Remove(key);

        engineServices.GetRequiredService<IQueueLocator>().Deregister(queueType, name);

        if (queue.Queue is IAsyncDisposable asyncDisposableQueue)
        {
            asyncDisposableQueue.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        else if (queue.Queue is IDisposable disposableQueue)
        {
            disposableQueue.Dispose();
        }

        if (queue.QueueScope is IAsyncDisposable asyncDisposableQueueScope)
        {
            asyncDisposableQueueScope.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
        else if (queue.QueueScope is IDisposable disposableQueueScope)
        {
            disposableQueueScope.Dispose();
        }
    }

    public void Dispose()
    {
        if (disposed) return;

        disposed = true;

        foreach (var queueKey in queues.Keys)
            RemoveQueueInner(queueKey.Type, queueKey.Name, checkDisposed: false);
    }

    public record struct EngineOptions
    {
        public static EngineOptions Default => new()
        {
            ConfigureEngineServices = x => { },
            DefaultConfigureQueueServices = x => { }
        };

        /// <summary>
        /// Manipultes services registered for the engine. These services are primarily used to 
        /// make new queues or to share state between queues. Note that the scope is per queue
        /// and stays over until the queue is removed and finishes shutting down.
        /// </summary>
        public Action<IServiceCollection> ConfigureEngineServices { get; set; }

        /// <summary>
        /// Manipultes the default services registered for created queues. These services are primarily 
        /// used in the execution of job. Note that the scope is per job and stays open until the job
        /// and all wrapping handlers finish executing.
        /// </summary>
        public Action<IServiceCollection> DefaultConfigureQueueServices { get; set; }

        public void Validate()
        {
            EnsureArg.IsNotNull(ConfigureEngineServices, nameof(ConfigureEngineServices));
            EnsureArg.IsNotNull(DefaultConfigureQueueServices, nameof(DefaultConfigureQueueServices));
        }
    }

    public record struct CreateQueueOptions
    {
        public static CreateQueueOptions Default => new()
        {
            Name = QueueLocator.DefaultQueueName,
            ConfigureQueueServices = x => { }
        };

        /// <summary>
        /// The name of the queue, primarily used to identify the queue when multiple queues of 
        /// the same type have been added.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Manipultes the services registered for created queues. These services are primarily 
        /// used in the execution of job. Note that the scope is per job and stays open until the job
        /// and all wrapping handlers finish executing.
        /// </summary>
        public Action<IServiceCollection> ConfigureQueueServices { get; set; }

        public void Validate()
        {
            EnsureArg.IsNotNull(Name, nameof(Name));
            EnsureArg.IsNotNull(ConfigureQueueServices, nameof(ConfigureQueueServices));
        }
    }

    private readonly record struct QueueInfo(IQueue Queue, IServiceScope QueueScope);

    private readonly record struct QueueKey(Type Type, string Name);
}
