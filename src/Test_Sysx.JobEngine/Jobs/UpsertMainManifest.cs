namespace Test_Sysx.JobEngine.Jobs;

public static class UpsertMainManifest
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
            if (!appAssetContext.Manifests.TryGet("main", out var _))
            {
                appAssetContext.Manifests.Add(new Manifest("main", Array.Empty<Guid>()));
            }
        }
    }
}
