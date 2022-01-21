namespace Test_Sysx.JobEngine.VoxelEngine;

public class ConfigurationAssetContext : AssetContext
{
    public IAssetSet<Guid, Pallet> Pallets { get; private set; } = null!;
}

public class ConfigurableAssetContext : AssetContext
{
    public IAssetSet<Guid, Pallet> Pallets { get; private set; } = null!;
}

public class MainAssetContext : ConfigurableAssetContext
{
    public IAssetSet<Guid, Grid> Grids { get; private set; } = null!;
    public IAssetSet<Guid, Chunk> Chunks { get; private set; } = null!;
}

public class ContouringAssetContext : ConfigurableAssetContext
{
    public IAssetSet<Guid, Shape> Shapes { get; private set; } = null!;
}