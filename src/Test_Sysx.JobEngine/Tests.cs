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
                services.AddSingleton<IAssetMapping>(new AssetMapping<string, Manifest>());
                services.AddTransient<IJobExecutor<UpsertMainManifest.Job>, UpsertMainManifest.Handler>();
                services.AddTransient<IJobExecutor<ReadMainManifest.Job>, ReadMainManifest.Handler>();
                services.AddTransient<IOnAddAssetEvent<Guid, Pallet>, OnAddOrUpsertAsset_RecordPalletToManifest>();
                services.AddTransient<IOnUpsertAssetEvent<Guid, Pallet>, OnAddOrUpsertAsset_RecordPalletToManifest>();
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
    }
}