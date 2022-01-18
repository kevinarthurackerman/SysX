namespace Test_Sysx.JobEngine.VoxelEngine;

public class ConfigurationAssetContext : AssetContext
{
    public ConfigurationAssetContext(IEnumerable<Type> assetTypes, IQueueServiceProvider queueServiceProvider)
        : base(assetTypes, queueServiceProvider)
    {
    }

    public IAssetSet<Guid, Pallet> Pallets { get; set; } = null!;
}

public class ConfigurableAssetContext : AssetContext
{
    public ConfigurableAssetContext(IEnumerable<Type> assetTypes, IQueueServiceProvider queueServiceProvider)
        : base(assetTypes, queueServiceProvider)
    {
    }

    public IAssetSet<Guid, Pallet> Pallets { get; set; } = null!;
}

public class MainAssetContext : ConfigurableAssetContext
{
    public MainAssetContext(IEnumerable<Type> assetTypes, IQueueServiceProvider queueServiceProvider)
        : base(assetTypes, queueServiceProvider)
    {
    }

    public IAssetSet<Guid, Grid> Grids { get; set; } = null!;
    public IAssetSet<Guid, Chunk> Chunks { get; set; } = null!;
}

public class ContouringAssetContext : ConfigurableAssetContext
{
    public ContouringAssetContext(IEnumerable<Type> assetTypes, IQueueServiceProvider queueServiceProvider)
        : base(assetTypes, queueServiceProvider)
    {
    }

    public IAssetSet<Guid, Shape> Shapes { get; set; } = null!;
}