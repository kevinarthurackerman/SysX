namespace SysX.JobEngine;

/// <summary>
/// A convenience service for propagating changes to assets in one queue to other queues.
/// An implementation of this must be registered in the queue from which asset changes will propagate.
/// A propagate assets job executor should be registered to all queues receiving propagated assets.
/// </summary>
public abstract class OnJobExecute_PropagateAssets<TJob, TJobExecutor> : IOnJobExecuteEvent<TJob, TJobExecutor>
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>
{
    private static readonly ConcurrentDictionary<Type, Action<IEnumerable<Type>, IEnumerable<AssetContext>, IEnumerable<IQueue>>> runPropagateAssetJobsCache = new();

    private readonly IQueueServiceProvider queueServiceProvider;
    private readonly IQueueContext queueContext;
    private readonly IQueueLocator queueLocator;
    private readonly IEnumerable<AssetContext> assetContexts;

    protected abstract IEnumerable<Type> FromContextTypes { get; }
    protected abstract IEnumerable<Type> ToQueueTypes { get; }
    protected abstract IEnumerable<Type> ToContextTypes { get; }
    protected abstract IEnumerable<Type> AssetTypes { get; }

    public OnJobExecute_PropagateAssets(IQueueServiceProvider queueServiceProvider)
    {
        EnsureArg.IsNotNull(queueServiceProvider, nameof(queueServiceProvider));

        foreach (var fromContextType in FromContextTypes)
        {
            EnsureArg.IsTrue(
                typeof(AssetContext).IsAssignableFrom(fromContextType),
                optsFn: x => x.WithMessage($"Type {fromContextType} from {nameof(FromContextTypes)} must be assignable to {typeof(AssetContext)}."));
        }

        foreach (var toQueueType in ToQueueTypes)
        {
            EnsureArg.IsTrue(
                typeof(IQueue).IsAssignableFrom(toQueueType),
                optsFn: x => x.WithMessage($"Type {toQueueType} from {nameof(ToQueueTypes)} must be assignable to {typeof(IQueue)}."));
        }

        foreach (var toContextType in ToContextTypes)
        {
            EnsureArg.IsTrue(
                typeof(AssetContext).IsAssignableFrom(toContextType),
                optsFn: x => x.WithMessage($"Type {toContextType} from {nameof(ToContextTypes)} must be assignable to {typeof(AssetContext)}."));
        }

        foreach (var toAssetType in AssetTypes)
        {
            EnsureArg.IsTrue(
                toAssetType.IsAssignableToGenericType(typeof(IAsset<>)),
                optsFn: x => x.WithMessage($"Type {toAssetType} from {nameof(AssetTypes)} must be assignable to {typeof(IAsset<>)}."));
        }

        this.queueServiceProvider = queueServiceProvider;
        queueContext = queueServiceProvider.GetRequiredService<IQueueContext>();
        queueLocator = queueServiceProvider.GetRequiredService<IQueueLocator>();
        assetContexts = FromContextTypes
            .Select(x => queueServiceProvider.GetService(x))
            .Where(x => x != null)
            .Cast<AssetContext>()
            .ToArray();
    }

    public OnJobExecuteEventResultData<TJob, TJobExecutor> Execute(in OnJobExecuteEventRequest<TJob, TJobExecutor> request, OnJobExecuteEventNext<TJob, TJobExecutor> next)
    {
        EnsureArg.HasValue(request, nameof(request));
        EnsureArg.IsNotNull(next, nameof(next));

        var result = next(request.Current);

        if (!assetContexts.Any()) return result.Current;

        var queues = queueLocator.GetAll()
            .Where(x => ToQueueTypes.Contains(x.GetType()) && x != queueContext.Current)
            .ToArray();

        if (!queues.Any()) return result.Current;

        foreach(var assetType in AssetTypes)
        {
            var runPropagateJob = runPropagateAssetJobsCache.GetOrAdd(assetType, assetType =>
            {
                var keyType = assetType.GetGenericTypeImplementation(typeof(IAsset<>))!
                    .GetGenericArguments()[0];

                var methodInfo = typeof(OnJobExecute_PropagateAssets<TJob, TJobExecutor>)
                    .GetMethod(nameof(PropagateAssets), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(keyType, assetType);

                return (IEnumerable<Type> toAssetContextTypes, IEnumerable<AssetContext> fromAssetContexts, IEnumerable<IQueue> propagateToQueues) =>
                    methodInfo.Invoke(null, new object[] { toAssetContextTypes, fromAssetContexts, propagateToQueues });
            });

            runPropagateJob(ToContextTypes, assetContexts, queues);
        }

        return result.Current;
    }

    private static void PropagateAssets<TKey, TAsset>(IEnumerable<Type> toAssetContextTypes, IEnumerable<AssetContext> fromAssetContexts, IEnumerable<IQueue> propagateToQueues)
        where TAsset : class, IAsset<TKey>
    {
        var modifiedAssetDatas = fromAssetContexts
            .Select(x => x.AssetSet<TKey, TAsset>())
            .SelectMany(x => x.GetUncommitted())
            .Select(x => new PropagateAssets<TKey, TAsset>.JobData.AssetData(x.Current, x.Uncommitted))
            .ToArray();

        foreach(var queue in propagateToQueues)
            queue.SubmitJob(new PropagateAssets<TKey, TAsset>.JobData(toAssetContextTypes, modifiedAssetDatas));
    }
}

/// <summary>
/// A convenience job for propagating changes to assets in one queue to other queues.
/// This executor should be registered to all queues receiving propagated assets.
/// An implementation of OnJobExecute_PropagateAssets must be registered in the queue from which asset changes will propagate.
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
                    foreach(var assetContext in assetContexts)
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