namespace Test_Sysx.JobEngine.VoxelEngine.Jobs;

public static class PropagatePallets
{
    public readonly record struct JobData(IEnumerable<JobData.PalletData> Pallets) : IJob
    {
        public readonly record struct PalletData(Pallet? Old, Pallet? New);
    };

    public class Executor : IJobExecutor<JobData>
    {
        private readonly ConfigurableAssetContext? configurableAssetContext;

        public Executor(ConfigurableAssetContext? configurableAssetContext)
        {
            this.configurableAssetContext = configurableAssetContext;
        }

        public void Execute(in JobData data)
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