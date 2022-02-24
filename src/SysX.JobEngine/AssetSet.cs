namespace SysX.JobEngine;

/// <summary>
/// A collection of assets that can be accessed within a job.
/// </summary>
internal class AssetSet<TKey, TAsset> : IAssetSet<TKey, TAsset>, ISinglePhaseNotification
    where TAsset : class, IAsset<TKey>
{
#pragma warning disable CS8714 // Key can be nullable in order to match asset key, but null will never be passed in as a key.
    private readonly IDictionary<TKey, TAsset> assets;
    private readonly IDictionary<TKey, TAsset?> uncommittedAssets;
#pragma warning restore CS8714 // Key can be nullable in order to match asset key, but null will never be passed in as a key.
    private Transaction? transaction;

    internal AssetSet()
    {
#pragma warning disable CS8714 // Key can be nullable in order to match asset key, but null will never be passed in as a key.
        assets = new Dictionary<TKey, TAsset>();
        uncommittedAssets = new Dictionary<TKey, TAsset?>();
#pragma warning restore CS8714 // Key can be nullable in order to match asset key, but null will never be passed in as a key.
    }

    /// <summary>
    /// Gets the asset from this context.
    /// </summary>
    public TAsset Get(TKey key)
    {
        EnsureArg.HasValue(key, nameof(key));

        var asset = Find(key);

        if (asset == null)
        {
            throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");
        }
        else
        {
            return asset;
        }
    }

    /// <summary>
    /// Tries to get the asset from this context.
    /// </summary>
    public bool TryGet(TKey key, out TAsset? result)
    {
        EnsureArg.HasValue(key, nameof(key));

        result = Find(key);

        return result != null;
    }

    /// <summary>
    /// Adds the asset to this context.
    /// </summary>
    public TAsset Add(TAsset asset)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        var existingAsset = Find(asset.Key);

        if (existingAsset == null)
        {
            Set(asset.Key, asset);
            return asset;
        }
        else
        {
            throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{asset.Key}' already exists.");
        }
    }

    /// <summary>
    /// Tries to add the asset to this context.
    /// </summary>
    public bool TryAdd(TAsset asset, out TAsset? result)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        var existingAsset = Find(asset.Key);

        if (existingAsset == null)
        {
            Set(asset.Key, asset);
            result = asset;
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Adds or updates the asset to this context.
    /// </summary>
    public TAsset AddOrUpdate(TAsset asset)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        Set(asset.Key, asset);
        return asset;
    }

    /// <summary>
    /// Tries to add or update the asset to this context.
    /// </summary>
    public bool TryAddOrUpdate(TAsset asset, out TAsset? result)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        Set(asset.Key, asset);
        result = asset;
        return true;
    }

    /// <summary>
    /// Updates the asset in this context.
    /// </summary>
    public TAsset Update(TAsset asset)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        var existingAsset = Find(asset.Key);

        if (existingAsset == null)
        {
            throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{asset.Key}' was not found.");
        }
        else
        {
            Set(asset.Key, asset);
            return asset;
        }
    }

    /// <summary>
    /// Tries to update the asset in this context.
    /// </summary>
    public bool TryUpdate(TAsset asset, out TAsset? result)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        var existingAsset = Find(asset.Key);

        if (existingAsset == null)
        {
            result = null;
            return false;
        }
        else
        {
            Set(asset.Key, asset);
            result = asset;
            return true;
        }
    }

    /// <summary>
    /// Removes the asset from this context.
    /// </summary>
    public TAsset Delete(TKey key)
    {
        EnsureArg.HasValue(key, nameof(key));

        EnlistTransaction();

        var asset = Find(key);

        if (asset == null)
        {
            throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");
        }
        else
        {
            Set(asset.Key, null);
            return asset;
        }
    }

    /// <summary>
    /// Tries to remove the asset from this context.
    /// </summary>
    public bool TryDelete(TKey key, out TAsset? result)
    {
        EnsureArg.HasValue(key, nameof(key));

        EnlistTransaction();

        result = Find(key);

        if (result == null)
        {
            return false;
        }
        else
        {
            Set(result.Key, null);
            return true;
        }
    }

    /// <summary>
    /// Gets an enumerable of assets with uncommitted changes which includes both
    /// the current version of the asset and the uncommitted change.
    /// </summary>
    public IEnumerable<UncommittedAsset<TKey, TAsset>> GetUncommitted()
    {
        foreach (var uncommitted in uncommittedAssets)
        {
            assets.TryGetValue(uncommitted.Key, out var current);

            if (current != uncommitted.Value)
                yield return new UncommittedAsset<TKey, TAsset>(current, uncommitted.Value);
        }
    }

    private TAsset? Find(TKey key)
    {
        if (Transaction.Current != null)
        {
            if (uncommittedAssets.TryGetValue(key, out var uncommittedAsset))
                return uncommittedAsset;
        }

        assets.TryGetValue(key, out var asset);

        return asset;
    }

    private void Set(TKey key, TAsset? asset)
    {
        if (Transaction.Current != null)
        {
            uncommittedAssets[key] = asset;
        }
        else
        {
            if (asset == null)
            {
                assets.Remove(key);
            }
            else
            {
                assets[key] = asset;
            }
        }
    }

    void ISinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
    {
        Commit();
        singlePhaseEnlistment.Committed();
    }

    void IEnlistmentNotification.Commit(Enlistment enlistment)
    {
        Commit();
        enlistment.Done();
    }

    void IEnlistmentNotification.InDoubt(Enlistment enlistment)
    {
        enlistment.Done();
    }

    void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
    {
        preparingEnlistment.Prepared();
    }

    void IEnlistmentNotification.Rollback(Enlistment enlistment)
    {
        transaction = null;

        uncommittedAssets.Clear();

        enlistment.Done();
    }

    private void EnlistTransaction()
    {
        if (Transaction.Current != null && Transaction.Current != transaction)
        {
            transaction = Transaction.Current;
            transaction.EnlistVolatile(this, EnlistmentOptions.None);
        }
    }

    private void Commit()
    {
        transaction = null;

        foreach (var asset in uncommittedAssets)
        {
            if (asset.Value == null)
            {
                assets.Remove(asset.Key);
            }
            else
            {
                assets[asset.Key] = asset.Value;
            }
        }

        uncommittedAssets.Clear();
    }
}
