namespace Test_SysX.JobEngine.App;

public class AppAssetContext : AssetContext
{
    public IAssetSet<string, Manifest> Manifests { get; } = null!;
}