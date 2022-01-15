namespace Sysx.JobEngine;

public class Engine
{
    private readonly Dictionary<QueueKey, QueueInfo> queues = new();
    private readonly IServiceCollection defaultQueueServices;
    private readonly IServiceProvider engineServices;

    public Engine(in CreateEngineOptions createEngineOptions)
    {
        createEngineOptions.Validate();

        queues = new();

        var engineServices = new ServiceCollection()
            .AddSingleton(engineServices =>
            {
                var internalServiceProvider = (IEngineServiceProvider)new InternalServiceProvider();
                internalServiceProvider.SetServiceProvider(engineServices);
                return internalServiceProvider;
            })
            .AddScoped<IQueueServiceProvider>(_ => new InternalServiceProvider())
            .AddScoped<IQueue, Queue>()
            .AddSingleton<IQueueLocator, QueueLocator>();

        createEngineOptions.ConfigureEngineServices(engineServices);

        this.engineServices = engineServices.BuildServiceProvider();

        var defaultQueueServices = new ServiceCollection()
            .AddSingleton(queueServiceProvider =>
            {
                var internalServiceProvider = (IQueueServiceProvider)new InternalServiceProvider();
                internalServiceProvider.SetServiceProvider(queueServiceProvider);
                return internalServiceProvider;
            })
            .AddSingleton(queueServiceProvider =>
            {
                return queueServiceProvider.GetRequiredService<IEngineServiceProvider>()
                    .GetRequiredService<IQueueLocator>();
            });

        createEngineOptions.DefaultConfigureQueueServices(defaultQueueServices);

        this.defaultQueueServices = Copy(defaultQueueServices);
    }

    public IQueue CreateQueue(in CreateQueueOptions createQueueOptions)
        => CreateQueue<IQueue>(in createQueueOptions);

    public TQueue CreateQueue<TQueue>(in CreateQueueOptions createQueueOptions)
        where TQueue : IQueue
    {
        var queueScope = engineServices.CreateScope();

        var queueServices = new ServiceCollection()
            .AddSingleton(_ => 
            {
                var internalServiceProvider = (IEngineServiceProvider)new InternalServiceProvider();
                internalServiceProvider.SetServiceProvider(queueScope.ServiceProvider);
                return internalServiceProvider;
            });

        foreach (var service in defaultQueueServices)
            queueServices.Add(service);

        createQueueOptions.ConfigureQueueServices(queueServices);

        var queueServiceProvider = queueServices.BuildServiceProvider();

        queueScope.ServiceProvider.GetRequiredService<IQueueServiceProvider>()
            .SetServiceProvider(queueServiceProvider);

        var queue = queueScope.ServiceProvider.GetRequiredService<TQueue>();

        queues.Add(new QueueKey(typeof(TQueue), createQueueOptions.Name), new QueueInfo(queue, queueScope));

        engineServices.GetRequiredService<IQueueLocator>()
            .RegisterQueue(queue, createQueueOptions.Name);

        return queue;
    }

    public void RemoveQueue()
        => RemoveQueue<IQueue>("Default");

    public void RemoveQueue(string name)
        => RemoveQueue<IQueue>(name);

    public void RemoveQueue<TQueue>()
        where TQueue : IQueue
        => RemoveQueue<TQueue>("Default");

    public void RemoveQueue<TQueue>(string name)
        where TQueue : IQueue
    {
        var key = new QueueKey(typeof(TQueue), name);

        var queue = queues[key];

        queues.Remove(new QueueKey(typeof(TQueue), name));

        engineServices.GetRequiredService<IQueueLocator>()
            .DeregisterQueue<TQueue>(name);

        if (queue.Queue is IDisposable disposable)
            disposable.Dispose();

        if (queue.Queue is IAsyncDisposable asyncDisposable)
            asyncDisposable.DisposeAsync().AsTask().GetAwaiter().GetResult();

        queue.QueueScope.Dispose();
    }

    private static IServiceCollection Copy(IServiceCollection services)
    {
        var copy = new ServiceCollection();

        foreach (var service in services)
            copy.Add(service);

        return copy;
    }

    public record struct CreateEngineOptions
    {
        public static CreateEngineOptions Default => new()
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
            Name = "Default",
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

    private readonly record struct QueueInfo(IQueue Queue, IDisposable QueueScope);

    private readonly record struct QueueKey(Type Type, string Name);
}
