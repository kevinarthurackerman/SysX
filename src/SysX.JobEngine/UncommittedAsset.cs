namespace SysX.JobEngine;

/// <summary>
/// The current and uncommitted versions of an uncommitted asset.
/// </summary>
public readonly record struct UncommittedAsset<TKey, TAsset>(TAsset? Current, TAsset? Uncommitted)
    where TAsset : class, IAsset<TKey>
{
    public TKey Key
    {
        get
        {
            if (Uncommitted != null) return Uncommitted.Key;
            if (Current != null) return Current.Key;

            throw new Exception($"An unexpected exception occurred. Both {nameof(Uncommitted)} and {nameof(Current)} were null.");
        }
    }
};
