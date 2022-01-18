namespace Test_Sysx.JobEngine.App.OnActionHandlers;

public class OnJobExecute_RecordPalletsToManifest<TJob, TJobExecutor> : IOnJobExecuteEvent<TJob, TJobExecutor>
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>
{
    private readonly ConfigurationAssetContext configurationAssetContext;
    private readonly AppAssetContext appAssetContext;

    public OnJobExecute_RecordPalletsToManifest(ConfigurationAssetContext configurationAssetContext, AppAssetContext appAssetContext)
    {
        this.configurationAssetContext = configurationAssetContext;
        this.appAssetContext = appAssetContext;
    }

    public OnJobExecuteEventResultData<TJob, TJobExecutor> Execute(in OnJobExecuteEventRequest<TJob, TJobExecutor> request, OnJobExecuteEventNext<TJob, TJobExecutor> next)
    {
        var result = next(request.Current);

        var modifiedPallets = configurationAssetContext.Pallets.GetUncommitted();

        if(!modifiedPallets.Any()) return result.Current;

        var addedPalletKeys = modifiedPallets
            .Where(x => x.Current == null)
            .Select(x => x.Key)
            .ToArray();

        var removedPalletKeys = modifiedPallets
            .Where(x => x.Uncommitted == null)
            .Select(x => x.Key)
            .ToArray();

        var manifest = appAssetContext.Manifests.Get("main");

        var newPalletIds = manifest.PalletIds
            .Except(removedPalletKeys)
            .Concat(addedPalletKeys)
            .ToArray();

        manifest = new Manifest(manifest.Key, newPalletIds);

        appAssetContext.Manifests.Update(manifest);

        return result.Current;
    }
}
