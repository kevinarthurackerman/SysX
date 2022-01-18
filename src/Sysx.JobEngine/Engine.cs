namespace Sysx.JobEngine;

public class Engine : IDisposable
{
    private readonly Dictionary<QueueKey, QueueInfo> queues;
    private readonly IServiceCollection defaultQueueServices;
    private readonly IServiceProvider engineServices;
    private bool disposed;

    public Engine(in EngineOptions createEngineOptions)
    {
        createEngineOptions.Validate();

        queues = new();
        disposed = false;

        var engineServices = new ServiceCollection()
            .AddScoped(engineServices => {
                var engineServiceProvider = (IEngineServiceProvider)new InternalServiceProvider();
                engineServiceProvider.SetServiceProvider(engineServices);
                return engineServiceProvider;
            })
            .AddScoped<IQueueServiceProvider>(_ => new InternalServiceProvider())
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

    public IQueue CreateQueue(in CreateQueueOptions createQueueOptions)
        => CreateQueue<IQueue>(in createQueueOptions);

    public TQueue CreateQueue<TQueue>(in CreateQueueOptions createQueueOptions)
        where TQueue : IQueue
    {
        Ensure.That(this).IsNotDisposed(disposed);

        createQueueOptions.Validate();

        var queueScope = engineServices.CreateScope();

        var queueServices = new ServiceCollection()
            .AddSingleton(_ => {
                var engineServiceProvider = (IEngineServiceProvider)new InternalServiceProvider();
                engineServiceProvider.SetServiceProvider(queueScope.ServiceProvider);
                return engineServiceProvider;
            })
            .AddSingleton(queueServices => {
                var queueServiceProvider = (IQueueServiceProvider)new InternalServiceProvider();
                queueServiceProvider.SetServiceProvider(queueServices);
                return queueServiceProvider;
            })
            .AddSingleton<IQueueContext, QueueContext>();

        foreach (var service in defaultQueueServices)
            queueServices.Add(service);

        createQueueOptions.ConfigureQueueServices(queueServices);

        var queueServiceProvider = queueServices.BuildServiceProvider();

        queueScope.ServiceProvider.GetRequiredService<IQueueServiceProvider>()
            .SetServiceProvider(queueServiceProvider);

        var queue = queueScope.ServiceProvider.GetRequiredService<TQueue>();

        queueServiceProvider.GetRequiredService<IQueueContext>().SetCurrent(queue);

        queues.Add(new QueueKey(typeof(TQueue), createQueueOptions.Name), new QueueInfo(queue, queueScope));

        engineServices.GetRequiredService<IQueueLocator>()
            .Register(queue, createQueueOptions.Name);

        return queue;
    }

    public void RemoveQueue()
        => RemoveQueueInner(typeof(IQueue), QueueLocator.DefaultQueueName, checkDisposed: true);

    public void RemoveQueue(string name)
        => RemoveQueueInner(typeof(IQueue), name, checkDisposed: true);

    public void RemoveQueue<TQueue>()
        where TQueue : IQueue
        => RemoveQueueInner(typeof(TQueue), QueueLocator.DefaultQueueName, checkDisposed: true);

    public void RemoveQueue<TQueue>(string name)
        where TQueue : IQueue
        => RemoveQueueInner(typeof(TQueue), name, checkDisposed: true);

    private void RemoveQueueInner(Type queueType, string name, bool checkDisposed)
    {
        if (checkDisposed) Ensure.That(this).IsNotDisposed(disposed);
        EnsureArg.IsTrue(
            typeof(IQueue).IsAssignableFrom(queueType),
            optsFn: x => x.WithMessage($"Type {nameof(queueType)} must be assignable to {typeof(IQueue)}"));
        EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

        name ??= QueueLocator.DefaultQueueName;
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

        public Action<IServiceCollection> ConfigureEngineServices { get; set; }
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

        public string Name { get; set; }
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
