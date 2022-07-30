namespace Test_SysX.JobEngine.VoxelEngine.Jobs;

public static class CreatePallet
{
	public readonly record struct JobData(Guid Id, ImmutableDictionary<int, Pallet.VoxelType> VoxelCodeMappings) : IJob;

	public class Executor : IJobExecutor<JobData>
	{
		private readonly ConfigurationAssetContext configAssetContext;

		public Executor(ConfigurationAssetContext configAssetContext)
		{
			this.configAssetContext = configAssetContext;
		}

		public void Execute(in JobData data)
		{
			var voxelPallet = new Pallet(data.Id, data.VoxelCodeMappings);

			configAssetContext.Pallets.AddOrUpdate(voxelPallet);
		}
	}
}