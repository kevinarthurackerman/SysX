namespace Test_Sysx.JobEngine;

public class AppAssetContext : AssetContext
{
    public AppAssetContext(IEnumerable<IAssetMapping> assetMappings, IQueueServiceProvider queueServiceProvider)
        : base(assetMappings, queueServiceProvider)
    {
    }

    public AssetSet<string, Manifest> Manifests { get; set; } = null!;
}