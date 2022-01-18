namespace Test_Sysx.JobEngine.App.Jobs;

public static class UpsertMainManifest
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
            if (!appAssetContext.Manifests.TryGet("main", out var _))
            {
                appAssetContext.Manifests.Add(new Manifest("main", Array.Empty<Guid>()));
            }
        }
    }
}
