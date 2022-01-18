namespace Test_Sysx.JobEngine.VoxelEngine.Jobs;

public static class PropagatePallets
{
    public readonly record struct Job(IEnumerable<Job.PalletData> Pallets) : IJob
    {
        public readonly record struct PalletData(Pallet? Old, Pallet? New);
    };

    public class Executor : IJobExecutor<Job>
    {
        private readonly ConfigurableAssetContext? configurableAssetContext;

        public Executor(ConfigurableAssetContext? configurableAssetContext)
        {
            this.configurableAssetContext = configurableAssetContext;
        }

        public void Execute(in Job data)
        {
            if (configurableAssetContext == null) return;

            foreach(var palletData in data.Pallets)
            {
                if (palletData.New == null)
                {
                    configurableAssetContext.Pallets.Delete(palletData.Old!.Key);
                }
                else
                {
                    configurableAssetContext.Pallets.Upsert(palletData.New!);
                }
            }
        }
    }
}