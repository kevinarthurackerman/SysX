namespace Test_Sysx.JobEngine;

public class Tests
{
    [Fact]
    public void Test()
    {
        var configuration = EngineFactory.CreateEngine(
            configureEngineServices: services => { },
            configureDefaultQueueServices: services => { },
            configureConfigQueueServices: services =>
            {
                services.AddOnAssetEvent(typeof(OnAddOrUpsertAsset_RecordPalletToManifest));

                services.AddSingleton<AppAssetContext>();
                services.AddSingleton<IAssetMapping>(new AssetMapping<string, Manifest>());
                services.AddJobExecutor(typeof(UpsertMainManifest.Executor));
                services.AddJobExecutor(typeof(ReadMainManifest.Executor));
            },
            configureMainQueueServices: services => { },
            configureContouringQueueServices: services => { });

        var voxelPalletId = Guid.NewGuid();

        configuration.ConfigQueue.SubmitJob(new UpsertMainManifest.Job());
            
        configuration.ConfigQueue.SubmitJob(new CreatePallet.Job
        { 
            Id = voxelPalletId,
            VoxelCodeMappings = ImmutableDictionary<int, Pallet.VoxelType>.Empty 
        });

        configuration.ConfigQueue.SubmitJob(new ReadMainManifest.Job());

        configuration.Engine.Dispose();
    }
}