namespace Test_Sysx.JobEngine.App.OnActionHandlers;

public class OnJobExecute_PropagatePallets<TJob, TJobExecutor> : IOnJobExecuteEvent<TJob, TJobExecutor>
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>
{
    private readonly ConfigurationAssetContext configurationAssetContext;
    private readonly MainQueue mainQueue;
    private readonly ContouringQueue contouringQueue;
    private readonly IQueueLocator queueLocator;

    public OnJobExecute_PropagatePallets(
        ConfigurationAssetContext configurationAssetContext,
        MainQueue mainQueue,
        ContouringQueue contouringQueue,
        IQueueLocator queueLocator)
    {
        this.configurationAssetContext = configurationAssetContext;
        this.mainQueue = mainQueue;
        this.contouringQueue = contouringQueue;
        this.queueLocator = queueLocator;
    }

    public OnJobExecuteEventResultData<TJob, TJobExecutor> Execute(in OnJobExecuteEventRequest<TJob, TJobExecutor> request, OnJobExecuteEventNext<TJob, TJobExecutor> next)
    {
        var result = next(request.Current);

        foreach (var palletModified in configurationAssetContext.Pallets.GetUncommitted())
        {
            mainQueue.SubmitJob(new PropagatePallet.Job(palletModified.Key, palletModified.Uncommitted));
            contouringQueue.SubmitJob(new PropagatePallet.Job(palletModified.Key, palletModified.Uncommitted));
            queueLocator.Get<MainQueue>("Background 1").SubmitJob(new PropagatePallet.Job(palletModified.Key, palletModified.Uncommitted));
            queueLocator.Get<MainQueue>("Background 2").SubmitJob(new PropagatePallet.Job(palletModified.Key, palletModified.Uncommitted));
        }

        return result.Current;
    }
}