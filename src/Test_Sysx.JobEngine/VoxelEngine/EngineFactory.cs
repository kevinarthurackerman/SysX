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
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Pallet>());
                services.AddSingleton<AssetContext>();
                services.AddSingleton(typeof(IOnGetAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnAddAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnUpsertAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnUpdateAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnDeleteAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                configureEngineServices(services);
            },
            DefaultConfigureQueueServices = services =>
            {
                services.AddSingleton<EngineContext>();
                services.AddSingleton(typeof(IOnGetAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnAddAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnUpsertAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnUpdateAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnDeleteAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                configureDefaultQueueServices(services);
            }
        });

        var configQueue = engine.CreateQueue(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddTransient<IJobExecutor<CreatePallet.Job>, CreatePallet.Handler>();
                configureConfigQueueServices(services);
            }
        });

        var mainQueue = engine.CreateQueue(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Grid>());
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Chunk>());
                services.AddSingleton<AssetContext>();
                configureMainQueueServices(services);
            }
        });

        var contouringQueue = engine.CreateQueue(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Shape>());
                services.AddSingleton<AssetContext>();
                configureContouringQueueServices(services);
            }
        });

        return new(engine, configQueue, mainQueue, contouringQueue);
    }

    public readonly record struct CreateEngineResult(Engine Engine, IQueue ConfigQueue, IQueue MainQueue, IQueue ContouringQueue);
}
