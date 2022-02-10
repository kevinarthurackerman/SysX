namespace Test_SysX.JobEngine.VoxelEngine;

public class ConfigurationAssetContext : AssetContext
{
    public IAssetSet<Guid, Pallet> Pallets { get; } = null!;
}

public class ConfigurableAssetContext : AssetContext
{
    public IAssetSet<Guid, Pallet> Pallets { get; } = null!;
}

public class MainAssetContext : ConfigurableAssetContext
{
    public IAssetSet<Guid, Grid> Grids { get; } = null!;
    public IAssetSet<Guid, Chunk> Chunks { get; } = null!;
}

public class ContouringAssetContext : ConfigurableAssetContext
{
    public IAssetSet<Guid, Shape> Shapes { get; } = null!;
}