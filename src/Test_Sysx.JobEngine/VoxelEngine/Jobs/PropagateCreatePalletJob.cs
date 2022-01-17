namespace Test_Sysx.JobEngine.VoxelEngine.Jobs;

public static class PropagateCreatePallet
{
    public readonly record struct Job(Pallet Pallet) : IJob;

    public class Handler : IJobExecutor<Job>
    {
        private readonly ConfigurableAssetContext configurableAssetContext;

        public Handler(ConfigurableAssetContext configurableAssetContext)
        {
            this.configurableAssetContext = configurableAssetContext;
        }

        public void Execute(in Job data)
        {
            configurableAssetContext.Pallets.Upsert(data.Pallet);
        }
    }
}