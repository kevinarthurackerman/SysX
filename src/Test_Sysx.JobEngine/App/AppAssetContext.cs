namespace Test_Sysx.JobEngine.App;

public class AppAssetContext : AssetContext
{
    public IAssetSet<string, Manifest> Manifests { get; set; } = null!;
}