namespace Test_Sysx.JobEngine.OnActionHandlers;

public class OnUpsertAsset_RecordPalletToManifest : IOnAddAssetEvent<Guid, Pallet>, IOnUpsertAssetEvent<Guid, Pallet>
{
    private readonly AssetContext assetContext;

    public OnUpsertAsset_RecordPalletToManifest(AssetContext assetContext)
    {
        this.assetContext = assetContext;
    }

    public OnAssetEventResultData<Guid, Pallet> Execute(in OnAssetEventRequest<Guid, Pallet> request, OnAssetEventNext<Guid, Pallet> next)
    {
        var result = next(request.Current);

        if (result.Current.Success && result.Current.Asset != null)
        {
            var manifest = assetContext.Manifests().Get("main");

            manifest = new Manifest(manifest.Key, manifest.PalletIds.Append(result.Current.Asset.Key).Distinct().ToArray());

            assetContext.Manifests().Update(manifest);
        }

        return result.Current;
    }
}
