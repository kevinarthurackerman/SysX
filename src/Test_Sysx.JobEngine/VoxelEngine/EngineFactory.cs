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
                services.AddScoped<ConfigQueue>();
                services.AddScoped<MainQueue>();
                services.AddScoped<ContouringQueue>();
                configureEngineServices(services);
            },
            DefaultConfigureQueueServices = services =>
            {
                services.AddSingleton(typeof(IOnGetAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnAddAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnUpsertAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnUpdateAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(typeof(IOnDeleteAssetEvent<,>), typeof(OnAssetEvent_Log<,>));
                services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IQueueLocator>().Get<ConfigQueue>());
                services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IQueueLocator>().Get<MainQueue>());
                services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IQueueLocator>().Get<ContouringQueue>());
                configureDefaultQueueServices(services);
            }
        });

        var configQueue = engine.CreateQueue<ConfigQueue>(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddSingleton<ConfigurationAssetContext>();
                services.AddSingleton<IAssetMapping>(new AssetMapping<Guid, Pallet>());
                services.AddTransient<IJobExecutor<CreatePallet.Job>, CreatePallet.Handler>();
                services.AddTransient<IOnAddAssetEvent<Guid, Pallet>, OnAddOrUpsertPallet_PropagatePallet>();
                services.AddTransient<IOnUpsertAssetEvent<Guid, Pallet>, OnAddOrUpsertPallet_PropagatePallet>();
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
                services.AddTransient<IJobExecutor<PropagateCreatePallet.Job>, PropagateCreatePallet.Handler>();
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
                services.AddTransient<IJobExecutor<PropagateCreatePallet.Job>, PropagateCreatePallet.Handler>();
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
                services.AddTransient<IJobExecutor<PropagateCreatePallet.Job>, PropagateCreatePallet.Handler>();
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
                services.AddTransient<IJobExecutor<PropagateCreatePallet.Job>, PropagateCreatePallet.Handler>();
                configureMainQueueServices(services);
            }
        });

        return new(engine, configQueue, mainQueue, contouringQueue, backgroundQueue1, backgroundQueue2);
    }

    public readonly record struct CreateEngineResult(Engine Engine, IQueue ConfigQueue, IQueue MainQueue, IQueue ContouringQueue, IQueue BackgroundQueue1, IQueue BackgroundQueue2);
}