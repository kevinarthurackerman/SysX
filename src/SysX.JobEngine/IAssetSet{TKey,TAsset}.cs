namespace SysX.JobEngine;

/// <summary>
/// A collection of assets that can be accessed within a job.
/// </summary>
public interface IAssetSet<TKey, TAsset> : IAssetSet
    where TAsset : class, IAsset<TKey>
{
    /// <summary>
    /// Gets the asset from this context.
    /// </summary>
    public TAsset Get(TKey key);

    /// <summary>
    /// Tries to get the asset from this context.
    /// </summary>
    public bool TryGet(TKey key, out TAsset? result);

    /// <summary>
    /// Adds the asset to this context.
    /// </summary>
    public TAsset Add(TAsset asset);

    /// <summary>
    /// Tries to add the asset to this context.
    /// </summary>
    public bool TryAdd(TAsset asset, out TAsset? result);

    /// <summary>
    /// Adds or updates the asset to this context.
    /// </summary>
    public TAsset AddOrUpdate(TAsset asset);

    /// <summary>
    /// Tries to add or update the asset to this context.
    /// </summary>
    public bool TryAddOrUpdate(TAsset asset, out TAsset? result);

    /// <summary>
    /// Updates the asset in this context.
    /// </summary>
    public TAsset Update(TAsset asset);

    /// <summary>
    /// Tries to update the asset in this context.
    /// </summary>
    public bool TryUpdate(TAsset asset, out TAsset? result);

    /// <summary>
    /// Removes the asset from this context.
    /// </summary>
    public TAsset Delete(TKey key);

    /// <summary>
    /// Tries to remove the asset from this context.
    /// </summary>
    public bool TryDelete(TKey key, out TAsset? result);

    /// <summary>
    /// Gets an enumerable of assets with uncommitted changes which includes both
    /// the current version of the asset and the uncommitted change.
    /// </summary>
    public IEnumerable<UncommittedAsset<TKey, TAsset>> GetUncommitted();
}
