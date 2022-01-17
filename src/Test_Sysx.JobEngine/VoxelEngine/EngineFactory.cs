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
        var engine = new Engine(Engine.CreateEngineOptions.Default with
        {
            ConfigureEngineServices = services =>
            {
                services.AddQueueToEngine(typeof(ConfigQueue));
                services.AddQueueToEngine(typeof(MainQueue));
                services.AddQueueToEngine(typeof(ContouringQueue));
                configureEngineServices(services);
            },
            DefaultConfigureQueueServices = services =>
            {
                services.AddOnAssetEvent(typeof(OnAssetEvent_Log<,>), ServiceLifetime.Singleton);
                services.AddQueueToQueue(typeof(ConfigQueue));
                services.AddQueueToQueue(typeof(MainQueue));
                services.AddQueueToQueue(typeof(ContouringQueue));
                configureDefaultQueueServices(services);
            }
        });

        var configQueue = engine.CreateQueue<ConfigQueue>(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddSingleton<ConfigurationAssetContext>();
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Pallet>());
                services.AddJobExecutor(typeof(CreatePallet.Handler));
                services.AddOnAssetEvent(typeof(OnAddOrUpsertPallet_PropagatePallet), ServiceLifetime.Singleton);
                configureConfigQueueServices(services);
            }
        });

        var mainQueue = engine.CreateQueue<MainQueue>(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddSingleton<MainAssetContext>();
                services.AddSingleton<ConfigurableAssetContext>(serviceProvider => serviceProvider.GetRequiredService<MainAssetContext>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Pallet>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Grid>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Chunk>());
                services.AddJobExecutor(typeof(PropagateCreatePallet.Handler));
                configureMainQueueServices(services);
            }
        });

        var contouringQueue = engine.CreateQueue<ContouringQueue>(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddSingleton<ContouringAssetContext>();
                services.AddSingleton<ConfigurableAssetContext>(serviceProvider => serviceProvider.GetRequiredService<ContouringAssetContext>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Pallet>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Shape>());
                services.AddJobExecutor(typeof(PropagateCreatePallet.Handler));
                configureContouringQueueServices(services);
            }
        });

        var backgroundQueue1 = engine.CreateQueue<MainQueue>(Engine.CreateQueueOptions.Default with
        {
            Name = "Background 1",
            ConfigureQueueServices = services =>
            {
                services.AddSingleton<MainAssetContext>();
                services.AddSingleton<ConfigurableAssetContext>(serviceProvider => serviceProvider.GetRequiredService<MainAssetContext>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Pallet>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Grid>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Chunk>());
                services.AddJobExecutor(typeof(PropagateCreatePallet.Handler));
                configureMainQueueServices(services);
            }
        });

        var backgroundQueue2 = engine.CreateQueue<MainQueue>(Engine.CreateQueueOptions.Default with
        {
            Name = "Background 2",
            ConfigureQueueServices = services =>
            {
                services.AddSingleton<MainAssetContext>();
                services.AddSingleton<ConfigurableAssetContext>(serviceProvider => serviceProvider.GetRequiredService<MainAssetContext>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Pallet>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Grid>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Chunk>());
                services.AddJobExecutor(typeof(PropagateCreatePallet.Handler));
                configureMainQueueServices(services);
            }
        });

        return new(engine, configQueue, mainQueue, contouringQueue, backgroundQueue1, backgroundQueue2);
    }

    public readonly record struct CreateEngineResult(Engine Engine, IQueue ConfigQueue, IQueue MainQueue, IQueue ContouringQueue, IQueue BackgroundQueue1, IQueue BackgroundQueue2);
}