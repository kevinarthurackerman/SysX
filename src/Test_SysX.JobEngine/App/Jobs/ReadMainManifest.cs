namespace Test_SysX.JobEngine.App.Jobs;

public static class ReadMainManifest
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
			var manifest = appAssetContext.Manifests.Get("main");
		}
	}
}