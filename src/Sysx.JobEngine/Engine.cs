namespace Sysx.JobEngine;

public class Engine
{
    private readonly List<QueueInfo> queues = new();
    private readonly IServiceCollection defaultQueueServices;
    private readonly IServiceProvider engineServices;

    public Engine(in CreateEngineOptions createEngineOptions)
    {
        createEngineOptions.Validate();

        queues = new();

        var engineServices = new ServiceCollection()
            .AddSingleton<IEngineServiceProvider>(engineServices =>
            {
                var internalServiceProvider = new InternalServiceProvider();
                internalServiceProvider.SetServiceProvider(engineServices);
                return internalServiceProvider;
            })
            .AddScoped<IQueueServiceProvider>(_ => new InternalServiceProvider())
            .AddScoped<IQueue, Queue>();

        createEngineOptions.ConfigureEngineServices(engineServices);

        this.engineServices = engineServices.BuildServiceProvider();

        var defaultQueueServices = new ServiceCollection();

        createEngineOptions.DefaultConfigureQueueServices(defaultQueueServices);

        this.defaultQueueServices = Copy(defaultQueueServices);
    }

    public IQueue CreateQueue(in CreateQueueOptions createQueueOptions)
    {
        var queueScope = engineServices.CreateScope();

        var queueServices = new ServiceCollection()
            .AddSingleton<IEngineServiceProvider>(_ => 
            {
                var internalServiceProvider = new InternalServiceProvider();
                internalServiceProvider.SetServiceProvider(engineServices);
                return internalServiceProvider;
            })
            .AddSingleton<IQueueServiceProvider>(queueServiceProvider => 
            {
                var internalServiceProvider = new InternalServiceProvider();
                internalServiceProvider.SetServiceProvider(queueServiceProvider);
                return internalServiceProvider;
            });

        foreach (var service in defaultQueueServices)
            queueServices.Add(service);

        createQueueOptions.ConfigureQueueServices(queueServices);

        var queueServiceProvider = queueServices.BuildServiceProvider();

        ((InternalServiceProvider)queueScope.ServiceProvider.GetRequiredService<IQueueServiceProvider>())
            .SetServiceProvider(queueServiceProvider);

        var queue = queueScope.ServiceProvider.GetRequiredService<IQueue>();

        queues.Add(new QueueInfo(queue, queueScope));

        return queue;
    }

    public void RemoveQueue(IQueue queue)
    {
        var queueInfo = queues
            .Where(x => x.Queue == queue)
            .Single();

        queueInfo.QueueScope.Dispose();

        queues.Remove(queueInfo);
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
            ConfigureQueueServices = x => { }
        };

        public Action<IServiceCollection> ConfigureQueueServices { get; set; }

        public void Validate()
        {
            EnsureArg.IsNotNull(ConfigureQueueServices, nameof(ConfigureQueueServices));
        }
    }

    private readonly record struct QueueInfo(IQueue Queue, IDisposable QueueScope);
}
