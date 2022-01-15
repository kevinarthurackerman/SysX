namespace Test_Sysx.JobEngine.OnActionHandlers;

internal class OnAddOrUpsertPallet_PropagatePallet : IOnAddAssetEvent<Guid, Pallet>, IOnUpsertAssetEvent<Guid, Pallet>
{
    private readonly MainQueue mainQueue;
    private readonly ContouringQueue contouringQueue;
    private readonly IQueueLocator queueLocator;

    public OnAddOrUpsertPallet_PropagatePallet(MainQueue mainQueue, ContouringQueue contouringQueue, IQueueLocator queueLocator)
    {
        this.mainQueue = mainQueue;
        this.contouringQueue = contouringQueue;
        this.queueLocator = queueLocator;
    }

    public OnAssetEventResultData<Guid, Pallet> Execute(in OnAssetEventRequest<Guid, Pallet> request, OnAssetEventNext<Guid, Pallet> next)
    {
        var result = next(request.Current);

        if (result.Current.Success && result.Current.Asset != null)
        {
            mainQueue.SubmitJob(new PropagateCreatePallet.Job(result.Current.Asset));
            contouringQueue.SubmitJob(new PropagateCreatePallet.Job(result.Current.Asset));
            queueLocator.Get<MainQueue>("Background 1").SubmitJob(new PropagateCreatePallet.Job(result.Current.Asset));
            queueLocator.Get<MainQueue>("Background 2").SubmitJob(new PropagateCreatePallet.Job(result.Current.Asset));
        }

        return result.Current;
    }
}