namespace Test_Sysx.JobEngine.OnActionHandlers;

internal class OnAddOrUpsertPallet_PropagatePallet : IOnAddAssetEvent<Guid, Pallet>, IOnUpsertAssetEvent<Guid, Pallet>
{
    private readonly IQueueLocator queueLocator;

    public OnAddOrUpsertPallet_PropagatePallet(IQueueLocator queueLocator)
    {
        this.queueLocator = queueLocator;
    }

    public OnAssetEventResultData<Guid, Pallet> Execute(in OnAssetEventRequest<Guid, Pallet> request, OnAssetEventNext<Guid, Pallet> next)
    {
        var result = next(request.Current);

        if (result.Current.Success && result.Current.Asset != null)
        {
            queueLocator.Get<MainQueue>().SubmitJob(new PropagateCreatePallet.Job(result.Current.Asset));
            queueLocator.Get<ContouringQueue>().SubmitJob(new PropagateCreatePallet.Job(result.Current.Asset));
            queueLocator.Get<MainQueue>("Background 1").SubmitJob(new PropagateCreatePallet.Job(result.Current.Asset));
            queueLocator.Get<MainQueue>("Background 2").SubmitJob(new PropagateCreatePallet.Job(result.Current.Asset));
        }

        return result.Current;
    }
}