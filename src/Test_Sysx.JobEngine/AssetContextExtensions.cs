namespace Test_Sysx.JobEngine;

public static class AssetContextExtensions
{
    public static AssetSet<string, Manifest> Manifests(this AssetContext assetContext) => new(assetContext);
}
