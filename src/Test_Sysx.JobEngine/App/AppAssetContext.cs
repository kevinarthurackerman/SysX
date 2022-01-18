namespace Test_Sysx.JobEngine.App;

public class AppAssetContext : AssetContext
{
    public AppAssetContext(IEnumerable<IAssetMapping> assetMappings, IQueueServiceProvider queueServiceProvider)
        : base(assetMappings, queueServiceProvider)
    {
    }

    public IAssetSet<string, Manifest> Manifests { get; set; } = null!;
}