namespace Test_Sysx.JobEngine.VoxelEngine.Jobs;

public static class PropagatePallet
{
    public readonly record struct Job(Guid PalletId, Pallet? Pallet) : IJob;

    public class Executor : IJobExecutor<Job>
    {
        private readonly ConfigurableAssetContext configurableAssetContext;

        public Executor(ConfigurableAssetContext configurableAssetContext)
        {
            this.configurableAssetContext = configurableAssetContext;
        }

        public void Execute(in Job data)
        {
            if (data.Pallet == null)
            {
                configurableAssetContext.Pallets.Delete(data.PalletId);
            }
            else
            {
                configurableAssetContext.Pallets.Upsert(data.Pallet);
            }
        }
    }
}