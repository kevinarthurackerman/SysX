namespace Test_Sysx.JobEngine.Jobs;

public static class ReadMainManifest
{
    public readonly record struct Job : IJob { }

    public class Handler : IJobExecutor<Job>
    {
        private readonly AssetContext assetContext;

        public Handler(AssetContext assetContext)
        {
            this.assetContext = assetContext;
        }

        public void Execute(in Job data)
        {
            var manifest = assetContext.Manifests().Get("main");
        }
    }
}