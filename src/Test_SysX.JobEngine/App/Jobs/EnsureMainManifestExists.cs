namespace Test_SysX.JobEngine.App.Jobs;

public static class EnsureMainManifestExists
{
	public readonly record struct JobData : IJob { }

	public class Executor : IJobExecutor<JobData>
	{
		private readonly AppAssetContext appAssetContext;

		public Executor(AppAssetContext appAssetContext)
		{
			this.appAssetContext = appAssetContext;
		}

		public void Execute(in JobData data)
		{
			if (!appAssetContext.Manifests.TryGet("main", out var _))
			{
				appAssetContext.Manifests.Add(new Manifest("main", Array.Empty<Guid>()));
			}
		}
	}
}
