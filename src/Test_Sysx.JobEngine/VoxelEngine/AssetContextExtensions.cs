namespace Test_Sysx.JobEngine.VoxelEngine;

public static class AssetContextExtensions
{
    public static AssetSet<Guid, Chunk> Chunks(this AssetContext assetContext) => assetContext.AssetSet<Guid, Chunk>();
    public static AssetSet<Guid, Grid> Grids(this AssetContext assetContext) => assetContext.AssetSet<Guid, Grid>();
    public static AssetSet<Guid, Pallet> Pallets(this AssetContext assetContext) => assetContext.AssetSet<Guid, Pallet>();
    public static AssetSet<Guid, Shape> Shapes(this AssetContext assetContext) => assetContext.AssetSet<Guid, Shape>();
}
