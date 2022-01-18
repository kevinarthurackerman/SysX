namespace Test_Sysx.JobEngine.VoxelEngine;

public static class EngineFactory
{
    public static CreateEngineResult CreateEngine(
        Action<IServiceCollection> configureEngineServices,
        Action<IServiceCollection> configureDefaultQueueServices,
        Action<IServiceCollection> configureConfigQueueServices,
        Action<IServiceCollection> configureMainQueueServices,
        Action<IServiceCollection> configureContouringQueueServices)
    {
        var engine = new Engine(Engine.EngineOptions.Default with
        {
            ConfigureEngineServices = services =>
            {
                services.AddQueueTypeToEngine(typeof(ConfigQueue));
                services.AddQueueTypeToEngine(typeof(MainQueue));
                services.AddQueueTypeToEngine(typeof(ContouringQueue));
                configureEngineServices(services);
            },
            DefaultConfigureQueueServices = services =>
            {
                services.AddOnJobExecute(typeof(OnJobExecute_Log<,>));
                services.AddQueueServiceToQueue(typeof(ConfigQueue));
                services.AddQueueServiceToQueue(typeof(MainQueue));
                services.AddQueueServiceToQueue(typeof(ContouringQueue));
                configureDefaultQueueServices(services);
            }
        });

        var configQueue = engine.CreateQueue<ConfigQueue>(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddAssetContext(typeof(ConfigurationAssetContext), new[] { typeof(Pallet) });
                services.AddJobExecutor(typeof(CreatePallet.Executor));
                services.AddOnJobExecute(typeof(OnJobExecute_PropagatePallets<,>));
                configureConfigQueueServices(services);
            }
        });

        var mainQueue = engine.CreateQueue<MainQueue>(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddAssetContext(typeof(MainAssetContext), new[] { typeof(Pallet), typeof(Grid), typeof(Chunk) });
                services.AddJobExecutor(typeof(PropagatePallet.Executor));
                configureMainQueueServices(services);
            }
        });

        var contouringQueue = engine.CreateQueue<ContouringQueue>(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddAssetContext(typeof(ContouringAssetContext), new[] { typeof(Pallet), typeof(Shape) });
                services.AddJobExecutor(typeof(PropagatePallet.Executor));
                configureContouringQueueServices(services);
            }
        });

        var backgroundQueue1 = engine.CreateQueue<MainQueue>(Engine.CreateQueueOptions.Default with
        {
            Name = "Background 1",
            ConfigureQueueServices = services =>
            {
                services.AddAssetContext(typeof(MainAssetContext), new[] { typeof(Pallet), typeof(Grid), typeof(Chunk) });
                services.AddJobExecutor(typeof(PropagatePallet.Executor));
                configureMainQueueServices(services);
            }
        });

        var backgroundQueue2 = engine.CreateQueue<MainQueue>(Engine.CreateQueueOptions.Default with
        {
            Name = "Background 2",
            ConfigureQueueServices = services =>
            {
                services.AddAssetContext(typeof(MainAssetContext), new[] { typeof(Pallet), typeof(Grid), typeof(Chunk) });
                services.AddJobExecutor(typeof(PropagatePallet.Executor));
                configureMainQueueServices(services);
            }
        });

        return new(engine, configQueue, mainQueue, contouringQueue, backgroundQueue1, backgroundQueue2);
    }

    public readonly record struct CreateEngineResult(Engine Engine, IQueue ConfigQueue, IQueue MainQueue, IQueue ContouringQueue, IQueue BackgroundQueue1, IQueue BackgroundQueue2);
}