namespace Test_Sysx.JobEngine.App.Jobs;

public static class ReadMainManifest
{
    public readonly record struct Job : IJob { }

    public class Executor : IJobExecutor<Job>
    {
        private readonly AppAssetContext appAssetContext;

        public Executor(AppAssetContext appAssetContext)
        {
            this.appAssetContext = appAssetContext;
        }

        public void Execute(in Job data)
        {
            var manifest = appAssetContext.Manifests.Get("main");
        }
    }
}