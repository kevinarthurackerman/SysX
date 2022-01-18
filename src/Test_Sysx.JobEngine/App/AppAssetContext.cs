namespace Test_Sysx.JobEngine.App;

public class AppAssetContext : AssetContext
{
    public AppAssetContext(IEnumerable<Type> assetTypes, IQueueServiceProvider queueServiceProvider)
        : base(assetTypes, queueServiceProvider)
    {
    }

    public IAssetSet<string, Manifest> Manifests { get; set; } = null!;
}