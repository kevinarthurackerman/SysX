namespace Test_Sysx.JobEngine.Jobs;

public static class ReadMainManifest
{
    public readonly record struct Job : IJob { }

    public class Handler : IJobExecutor<Job>
    {
        private readonly AssetContext assetContext;

        public Handler(IEngineServiceProvider engineServiceProvider)
        {
            assetContext = engineServiceProvider.GetRequiredService<AssetContext>();
        }

        public void Execute(in Job data)
        {
            var manifest = assetContext.GetAsset<Manifest>("main");
        }
    }
}