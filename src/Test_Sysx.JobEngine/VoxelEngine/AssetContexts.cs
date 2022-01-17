namespace Test_Sysx.JobEngine.VoxelEngine;

public class ConfigurationAssetContext : AssetContext
{
    public ConfigurationAssetContext(IEnumerable<IAssetMapping> assetMappings, IQueueServiceProvider queueServiceProvider)
        : base(assetMappings, queueServiceProvider)
    {
    }

    public AssetSet<Guid, Pallet> Pallets { get; set; } = null!;
}

public class ConfigurableAssetContext : AssetContext
{
    public ConfigurableAssetContext(IEnumerable<IAssetMapping> assetMappings, IQueueServiceProvider queueServiceProvider)
        : base(assetMappings, queueServiceProvider)
    {
    }

    public AssetSet<Guid, Pallet> Pallets { get; set; } = null!;
}

public class MainAssetContext : ConfigurableAssetContext
{
    public MainAssetContext(IEnumerable<IAssetMapping> assetMappings, IQueueServiceProvider queueServiceProvider)
        : base(assetMappings, queueServiceProvider)
    {
    }

    public AssetSet<Guid, Grid> Grids { get; set; } = null!;
    public AssetSet<Guid, Chunk> Chunks { get; set; } = null!;
}

public class ContouringAssetContext : ConfigurableAssetContext
{
    public ContouringAssetContext(IEnumerable<IAssetMapping> assetMappings, IQueueServiceProvider queueServiceProvider)
        : base(assetMappings, queueServiceProvider)
    {
    }

    public AssetSet<Guid, Shape> Shapes { get; set; } = null!;
}