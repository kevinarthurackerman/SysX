namespace Test_Sysx.JobEngine;

public class Tests
{
    [Fact]
    public void Test()
    {
        var configuration = EngineFactory.CreateEngine(
            configureEngineServices: x =>
            {
                x.AddSingleton<IAssetMapping>(new AssetMapping<Manifest, string>(x => x.Id));
                x.AddTransient<IOnAddAssetEvent<Pallet>, OnUpsertAsset_RecordPalletToManifest>();
                x.AddTransient<IOnUpsertAssetEvent<Pallet>, OnUpsertAsset_RecordPalletToManifest>();
            },
            configureDefaultQueueServices: x => { },
            configureConfigQueueServices: x =>
            {
                x.AddTransient<IJobExecutor<UpsertMainManifest.Job>, UpsertMainManifest.Handler>();
                x.AddTransient<IJobExecutor<ReadMainManifest.Job>, ReadMainManifest.Handler>();
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