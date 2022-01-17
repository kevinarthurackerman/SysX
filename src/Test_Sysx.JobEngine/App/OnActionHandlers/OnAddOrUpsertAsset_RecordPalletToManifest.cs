namespace Test_Sysx.JobEngine.App.OnActionHandlers;

public class OnAddOrUpsertAsset_RecordPalletToManifest : IOnAddAssetEvent<Guid, Pallet>, IOnUpsertAssetEvent<Guid, Pallet>
{
    private readonly AppAssetContext appAssetContext;

    public OnAddOrUpsertAsset_RecordPalletToManifest(AppAssetContext appAssetContext)
    {
        this.appAssetContext = appAssetContext;
    }

    public OnAssetEventResultData<Guid, Pallet> Execute(in OnAssetEventRequest<Guid, Pallet> request, OnAssetEventNext<Guid, Pallet> next)
    {
        var result = next(request.Current);

        if (result.Current.Success && result.Current.Asset != null)
        {
            var manifest = appAssetContext.Manifests.Get("main");

            manifest = new Manifest(manifest.Key, manifest.PalletIds.Append(result.Current.Asset.Key).Distinct().ToArray());

            appAssetContext.Manifests.Update(manifest);
        }

        return result.Current;
    }
}
