namespace Test_Sysx.JobEngine.VoxelEngine;

public static class AssetContextExtensions
{
    public static AssetSet<Guid, Chunk> Chunks(this AssetContext assetContext) => new(assetContext);
    public static AssetSet<Guid, Grid> Grids(this AssetContext assetContext) => new(assetContext);
    public static AssetSet<Guid, Pallet> Pallets(this AssetContext assetContext) => new(assetContext);
    public static AssetSet<Guid, Shape> Shapes(this AssetContext assetContext) => new(assetContext);

    public readonly record struct AssetSet<TKey, TAsset>
        where TAsset : class, IAsset<TKey>
    {
        private readonly AssetContext assetContext;

        public AssetSet(AssetContext assetContext)
        {
            this.assetContext = assetContext;
        }

        public TAsset Get(TKey key) => assetContext.GetAsset<TKey, TAsset>(key);
        public bool TryGet(TKey key, out TAsset? result) => assetContext.TryGetAsset<TKey, TAsset>(key, out result);
        public TAsset Add(TAsset asset) => assetContext.AddAsset<TKey, TAsset>(asset);
        public bool TryAdd(TAsset asset, out TAsset? result) => assetContext.TryAddAsset<TKey, TAsset>(asset, out result);
        public TAsset Upsert(TAsset asset) => assetContext.UpsertAsset<TKey, TAsset>(asset);
        public bool TryUpsert(TAsset asset, out TAsset? result) => assetContext.TryUpsertAsset<TKey, TAsset>(asset, out result);
        public TAsset Update(TAsset asset) => assetContext.UpdateAsset<TKey, TAsset>(asset);
        public bool TryUpdate(TAsset asset, out TAsset? result) => assetContext.TryUpdateAsset<TKey, TAsset>(asset, out result);
        public TAsset Delete(TKey key) => assetContext.DeleteAsset<TKey, TAsset>(key);
        public bool TryDelete(TKey key, out TAsset? result) => assetContext.TryDeleteAsset<TKey, TAsset>(key, out result);
    }
}
