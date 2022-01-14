namespace Test_Sysx.JobEngine.VoxelEngine.Jobs;

public static class CreatePallet
{
    public readonly record struct Job(Guid Id, ImmutableDictionary<int, Pallet.VoxelType> VoxelCodeMappings) : IJob;

    public class Handler : IJobExecutor<Job>
    {
        private readonly EngineContext engineContext;

        public Handler(EngineContext engineContext)
        {
            this.engineContext = engineContext;
        }

        public void Execute(in Job data)
        {
            var voxelPallet = new Pallet(data.Id, data.VoxelCodeMappings);

            engineContext.AssetContext.Pallets().Upsert(voxelPallet);
        }
    }
}