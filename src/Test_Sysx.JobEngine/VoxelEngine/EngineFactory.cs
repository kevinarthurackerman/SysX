namespace Test_SysX.JobEngine.VoxelEngine;

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
                services.AddAssetContext(typeof(ConfigurationAssetContext));
                services.AddJobExecutor(typeof(CreatePallet.Executor));
                //services.AddOnJobExecute(typeof(OnJobExecute_PropagatePallets<,>));
                services.AddOnJobExecute(typeof(OnJobExecute_PropagatePallets<,>));
                configureConfigQueueServices(services);
            }
        });

        var mainQueue = engine.CreateQueue<MainQueue>(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddAssetContext(typeof(MainAssetContext));
                //services.AddJobExecutor(typeof(PropagatePallets.Executor));
                services.AddJobExecutor(typeof(PropagateAssets<Guid,Pallet>.Executor));
                configureMainQueueServices(services);
            }
        });

        var contouringQueue = engine.CreateQueue<ContouringQueue>(Engine.CreateQueueOptions.Default with
        {
            ConfigureQueueServices = services =>
            {
                services.AddAssetContext(typeof(ContouringAssetContext));
                //services.AddJobExecutor(typeof(PropagatePallets.Executor));
                services.AddJobExecutor(typeof(PropagateAssets<Guid, Pallet>.Executor));
                configureContouringQueueServices(services);
            }
        });

        var backgroundQueue1 = engine.CreateQueue<MainQueue>(Engine.CreateQueueOptions.Default with
        {
            Name = "Background 1",
            ConfigureQueueServices = services =>
            {
                services.AddAssetContext(typeof(MainAssetContext));
                //services.AddJobExecutor(typeof(PropagatePallets.Executor));
                services.AddJobExecutor(typeof(PropagateAssets<Guid, Pallet>.Executor));
                configureMainQueueServices(services);
            }
        });

        var backgroundQueue2 = engine.CreateQueue<MainQueue>(Engine.CreateQueueOptions.Default with
        {
            Name = "Background 2",
            ConfigureQueueServices = services =>
            {
                services.AddAssetContext(typeof(MainAssetContext));
                //services.AddJobExecutor(typeof(PropagatePallets.Executor));
                services.AddJobExecutor(typeof(PropagateAssets<Guid, Pallet>.Executor));
                configureMainQueueServices(services);
            }
        });

        return new(engine, configQueue, mainQueue, contouringQueue, backgroundQueue1, backgroundQueue2);
    }

    public readonly record struct CreateEngineResult(Engine Engine, IQueue ConfigQueue, IQueue MainQueue, IQueue ContouringQueue, IQueue BackgroundQueue1, IQueue BackgroundQueue2);
}