namespace Test_Sysx.JobEngine.App.OnActionHandlers;

public class OnJobExecute_PropagatePallets<TJob, TJobExecutor> : IOnJobExecuteEvent<TJob, TJobExecutor>
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>
{
    private readonly IQueueContext queueContext;
    private readonly ConfigurationAssetContext configurationAssetContext;
    private readonly IQueueLocator queueLocator;

    public OnJobExecute_PropagatePallets(
        IQueueContext queueContext,
        ConfigurationAssetContext configurationAssetContext,
        IQueueLocator queueLocator)
    {
        this.queueContext = queueContext;
        this.configurationAssetContext = configurationAssetContext;
        this.queueLocator = queueLocator;
    }

    public OnJobExecuteEventResultData<TJob, TJobExecutor> Execute(in OnJobExecuteEventRequest<TJob, TJobExecutor> request, OnJobExecuteEventNext<TJob, TJobExecutor> next)
    {
        var result = next(request.Current);

        var palletsModified = configurationAssetContext.Pallets.GetUncommitted();

        if (!palletsModified.Any()) return result.Current;

        var queues = queueLocator.GetAll()
            .Where(x => x != queueContext.Current)
            .ToArray();

        foreach (var queue in queues)
        {
            var palletDatas = palletsModified
                .Select(x => new PropagatePallets.JobData.PalletData(x.Current, x.Uncommitted))
                .ToArray();

            queue.SubmitJob(new PropagatePallets.JobData(palletDatas));
        }

        return result.Current;
    }
}