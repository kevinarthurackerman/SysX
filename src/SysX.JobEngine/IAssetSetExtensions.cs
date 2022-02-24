namespace SysX.JobEngine;

public static class IAssetSetExtensions
{
    /// <summary>
    /// Gets the version of the given asset that is stored in this context.
    /// </summary>
    public static TAsset Get<TKey, TAsset>(
        this IAssetSet<TKey, TAsset> assetSet, TAsset asset)
        where TAsset : class, IAsset<TKey>
        => assetSet.Get(asset.Key);

    /// <summary>
    /// Tries to get the version of the given asset that is stored in this context.
    /// </summary>
    public static bool TryGet<TKey, TAsset>(
        this IAssetSet<TKey, TAsset> assetSet, TAsset asset, out TAsset? result)
        where TAsset : class, IAsset<TKey>
        => assetSet.TryGet(asset.Key, out result);

    /// <summary>
    /// Removes the asset from this context.
    /// </summary>
    public static TAsset Delete<TKey, TAsset>(
        this IAssetSet<TKey, TAsset> assetSet, TAsset asset)
        where TAsset : class, IAsset<TKey>
        => assetSet.Delete(asset.Key);

    /// <summary>
    /// Tries to remove the asset from this context.
    /// </summary>
    public static bool TryDelete<TKey, TAsset>(
        this IAssetSet<TKey, TAsset> assetSet, TAsset asset, out TAsset? result)
        where TAsset : class, IAsset<TKey>
        => assetSet.TryDelete(asset.Key, out result);
}
