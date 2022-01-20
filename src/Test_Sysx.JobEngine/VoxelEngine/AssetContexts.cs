namespace Test_Sysx.JobEngine.VoxelEngine;

public class ConfigurationAssetContext : AssetContext
{
    public IAssetSet<Guid, Pallet> Pallets { get; set; } = null!;
}

public class ConfigurableAssetContext : AssetContext
{
    public IAssetSet<Guid, Pallet> Pallets { get; set; } = null!;
}

public class MainAssetContext : ConfigurableAssetContext
{
    public IAssetSet<Guid, Grid> Grids { get; set; } = null!;
    public IAssetSet<Guid, Chunk> Chunks { get; set; } = null!;
}

public class ContouringAssetContext : ConfigurableAssetContext
{
    public IAssetSet<Guid, Shape> Shapes { get; set; } = null!;
}