namespace Test_Sysx.JobEngine.VoxelEngine.Jobs;

public static class CreatePallet
{
    public readonly record struct Job(Guid Id, ImmutableDictionary<int, Pallet.VoxelType> VoxelCodeMappings) : IJob;

    public class Handler : IJobExecutor<Job>
    {
        private readonly AssetContext assetContext;

        public Handler(AssetContext assetContext)
        {
            this.assetContext = assetContext;
        }

        public void Execute(in Job data)
        {
            var voxelPallet = new Pallet(data.Id, data.VoxelCodeMappings);

            assetContext.Pallets().Upsert(voxelPallet);
        }
    }
}