namespace Test_SysX.JobEngine;

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
                services.AddOnJobExecute(typeof(OnJobExecuteRecordPalletsToManifest<,>));

                services.AddAssetContext(typeof(AppAssetContext));
                services.AddJobExecutor(typeof(EnsureMainManifestExists.Executor));
                services.AddJobExecutor(typeof(ReadMainManifest.Executor));
            },
            configureMainQueueServices: services => { },
            configureContouringQueueServices: services => { });

        configuration.ConfigQueue.SubmitJob(new EnsureMainManifestExists.JobData());
            
        configuration.ConfigQueue.SubmitJob(new CreatePallet.JobData
        { 
            Id = Guid.NewGuid(),
            VoxelCodeMappings = ImmutableDictionary<int, Pallet.VoxelType>.Empty 
        });

        configuration.ConfigQueue.SubmitJob(new ReadMainManifest.JobData());

        configuration.Engine.Dispose();
    }
}