namespace Test_Sysx.JobEngine.Jobs;

public static class ReadMainManifest
{
    public readonly record struct Job : IJob { }

    public class Handler : IJobExecutor<Job>
    {
        private readonly AppAssetContext appAssetContext;

        public Handler(AppAssetContext appAssetContext)
        {
            this.appAssetContext = appAssetContext;
        }

        public void Execute(in Job data)
        {
            var manifest = appAssetContext.Manifests.Get("main");
        }
    }
}