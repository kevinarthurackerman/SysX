namespace Test_Sysx.JobEngine.VoxelEngine.Jobs;

public static class PropagateCreatePallet
{
    public readonly record struct Job(Pallet Pallet) : IJob;

    public class Handler : IJobExecutor<Job>
    {
        private readonly AssetContext assetContext;

        public Handler(AssetContext assetContext)
        {
            this.assetContext = assetContext;
        }

        public void Execute(in Job data)
        {
            assetContext.Pallets().Upsert(data.Pallet);
        }
    }
}