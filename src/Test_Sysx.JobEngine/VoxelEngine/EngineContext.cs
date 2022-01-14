namespace Test_Sysx.JobEngine.VoxelEngine;

public class EngineContext
{
    public IServiceProvider ServiceProvider { get; }
    public AssetContext AssetContext { get; }

    public EngineContext(IEngineServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        AssetContext = serviceProvider.GetRequiredService<AssetContext>();
    }
}
