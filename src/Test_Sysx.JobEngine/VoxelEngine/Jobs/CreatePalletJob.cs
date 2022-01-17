namespace Test_Sysx.JobEngine.VoxelEngine.Jobs;

public static class CreatePallet
{
    public readonly record struct Job(Guid Id, ImmutableDictionary<int, Pallet.VoxelType> VoxelCodeMappings) : IJob;

    public class Handler : IJobExecutor<Job>
    {
        private readonly ConfigurationAssetContext configAssetContext;

        public Handler(ConfigurationAssetContext configAssetContext)
        {
            this.configAssetContext = configAssetContext;
        }

        public void Execute(in Job data)
        {
            var voxelPallet = new Pallet(data.Id, data.VoxelCodeMappings);

            configAssetContext.Pallets.Upsert(voxelPallet);
        }
    }
}