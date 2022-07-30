namespace SysX.JobEngine;

/// <summary>
/// A convenience job for propagating changes to assets in one queue to other queues.
/// This executor should be registered to all queues receiving propagated assets.
/// An implementation of <see cref="OnJobExecutePropagateAssets{TJob, TJobExecutor}"/>
/// must be registered in the queue from which asset changes will propagate.
/// </summary>
public static class PropagateAssets<TKey, TAsset>
	where TAsset : class, IAsset<TKey>
{
	public readonly record struct JobData(IEnumerable<Type> ToAssetContextTypes, IEnumerable<JobData.AssetData> Assets) : IJob
	{
		public readonly record struct AssetData(TAsset? Old, TAsset? New);
	};

	public class Executor : IJobExecutor<JobData>
	{
		private readonly IQueueServiceProvider queueServiceProvider;

		public Executor(IQueueServiceProvider queueServiceProvider)
		{
			EnsureArg.IsNotNull(queueServiceProvider, nameof(queueServiceProvider));

			this.queueServiceProvider = queueServiceProvider;
		}

		public void Execute(in JobData data)
		{
			EnsureArg.HasValue(data, nameof(data));

			var assetContexts = data.ToAssetContextTypes
				.Select(x => queueServiceProvider.GetService(x))
				.Where(x => x != null)
				.Cast<AssetContext>()
				.ToArray();

			if (!assetContexts.Any()) return;

			foreach (var assetData in data.Assets)
			{
				if (assetData.New == null)
				{
					foreach (var assetContext in assetContexts)
						assetContext.AssetSet<TKey, TAsset>().Delete(assetData.Old!.Key);
				}
				else
				{
					foreach (var assetContext in assetContexts)
						assetContext.AssetSet<TKey, TAsset>().AddOrUpdate(assetData.New!);
				}
			}
		}
	}
}
