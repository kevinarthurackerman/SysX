namespace Test_SysX.JobEngine.App.OnActionHandlers;

public class OnJobExecute_PropagatePallets<TJob, TJobExecutor> : OnJobExecute_PropagateAssets<TJob, TJobExecutor>
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>
{
    public OnJobExecute_PropagatePallets(IQueueServiceProvider queueServiceProvider) : base(queueServiceProvider)
    {
    }

    protected override IEnumerable<Type> FromContextTypes { get; } = new[] { typeof(ConfigurationAssetContext) };

    protected override IEnumerable<Type> ToQueueTypes { get; } = new[] { typeof(MainQueue), typeof(ContouringQueue) };

    protected override IEnumerable<Type> ToContextTypes { get; } = new[] { typeof(ConfigurableAssetContext) };

    protected override IEnumerable<Type> AssetTypes { get; } = new[] { typeof(Pallet) };
}

//public class OnJobExecute_PropagatePallets<TJob, TJobExecutor> : IOnJobExecuteEvent<TJob, TJobExecutor>
//    where TJob : IJob
//    where TJobExecutor : IJobExecutor<TJob>
//{
//    private readonly IQueueContext queueContext;
//    private readonly ConfigurationAssetContext configurationAssetContext;
//    private readonly IQueueLocator queueLocator;

//    public OnJobExecute_PropagatePallets(
//        IQueueContext queueContext,
//        ConfigurationAssetContext configurationAssetContext,
//        IQueueLocator queueLocator)
//    {
//        this.queueContext = queueContext;
//        this.configurationAssetContext = configurationAssetContext;
//        this.queueLocator = queueLocator;
//    }

//    public OnJobExecuteEventResultData<TJob, TJobExecutor> Execute(in OnJobExecuteEventRequest<TJob, TJobExecutor> request, OnJobExecuteEventNext<TJob, TJobExecutor> next)
//    {
//        var result = next(request.Current);

//        var palletsModified = configurationAssetContext.Pallets.GetUncommitted();

//        if (!palletsModified.Any()) return result.Current;

//        var queues = queueLocator.GetAll()
//            .Where(x => x != queueContext.Current)
//            .ToArray();

//        foreach (var queue in queues)
//        {
//            var palletDatas = palletsModified
//                .Select(x => new PropagatePallets.JobData.PalletData(x.Current, x.Uncommitted))
//                .ToArray();

//            queue.SubmitJob(new PropagatePallets.JobData(palletDatas));
//        }

//        return result.Current;
//    }
//}