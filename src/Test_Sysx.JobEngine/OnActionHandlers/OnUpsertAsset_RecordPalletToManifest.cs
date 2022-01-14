namespace Test_Sysx.JobEngine.OnActionHandlers;

public class OnUpsertAsset_RecordPalletToManifest : IOnAddAssetEvent<Pallet>, IOnUpsertAssetEvent<Pallet>
{
    private readonly AssetContext assetContext;

    public OnUpsertAsset_RecordPalletToManifest(IEngineServiceProvider engineServiceProvider)
    {
        assetContext = engineServiceProvider.GetRequiredService<AssetContext>();
    }

    public OnAssetEventResultData Execute(in OnAssetEventRequest request, OnAssetEventNext next)
    {
        var result = next(request.Current);

        var pallet = (Pallet)result.Current.Asset;

        var manifest = assetContext.GetAsset<Manifest>("main");

        manifest = new Manifest(manifest.Id, manifest.PalletIds.Append(pallet.Id).Distinct().ToArray());

        assetContext.UpsertAsset(manifest);

        return result.Current;
    }
}
