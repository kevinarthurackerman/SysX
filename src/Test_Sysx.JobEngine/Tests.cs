namespace Test_Sysx.JobEngine;

public class Tests
{
    [Fact]
    public void Test()
    {
        var configuration = EngineFactory.CreateEngine(
            configureEngineServices: x => { },
            configureDefaultQueueServices: x => { },
            configureConfigQueueServices: x =>
            {
                x.AddSingleton<IAssetMapping>(new AssetMapping<string, Manifest>());
                x.AddTransient<IJobExecutor<UpsertMainManifest.Job>, UpsertMainManifest.Handler>();
                x.AddTransient<IJobExecutor<ReadMainManifest.Job>, ReadMainManifest.Handler>();
                x.AddTransient<IOnAddAssetEvent<Guid, Pallet>, OnUpsertAsset_RecordPalletToManifest>();
                x.AddTransient<IOnUpsertAssetEvent<Guid, Pallet>, OnUpsertAsset_RecordPalletToManifest>();
            },
            configureMainQueueServices: x => { },
            configureContouringQueueServices: x => { });

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