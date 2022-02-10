namespace SysX.JobEngine;

/// <summary>
/// Acts as an in-memory repository for assets that are accessed by jobs.
/// 
/// Assets can either be accessed by calling the AssetSet method with the type of asset, 
/// or by adding a property to the class of type IAssetSet with a public getter and public or private setter.
/// </summary>
public abstract class AssetContext
{
    private readonly Dictionary<Type, IAssetSet> assetSetCache;

    public AssetContext() : this(Type.EmptyTypes) { }

    public AssetContext(IEnumerable<Type> assetTypes)
    {
        EnsureArg.IsNotNull(assetTypes, nameof(assetTypes));

        assetSetCache = assetTypes.ToDictionary(assetType => assetType, assetType => CreateAssetSet(assetType));

        foreach (var property in GetType().GetProperties())
        {
            var iassetSet = property.PropertyType
                .GetGenericTypeImplementation(typeof(IAssetSet<,>));

            if (iassetSet == null) continue;

            var assetType = iassetSet.GenericTypeArguments[1];

            if (!assetSetCache.TryGetValue(assetType, out var assetSet))
                assetSetCache[assetType] = assetSet = CreateAssetSet(assetType);

            property.SetBackingValue(this, assetSetCache[assetType]);
        }

        IAssetSet CreateAssetSet(Type assetType)
        {
            var iasset = assetType.GetGenericTypeImplementation(typeof(IAsset<>));

            if (iasset == null)
                throw new InvalidCastException($"Type {assetType} does not implement {typeof(IAsset<>)}.");

            var assetKeyType = iasset.GenericTypeArguments[0];

            return (IAssetSet)typeof(AssetSet<,>)
                .MakeGenericType(assetKeyType, assetType)
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single()
                .Invoke(null);
        }
    }

    /// <summary>
    /// Used to access assets of the given type stored in this context.
    /// </summary>
    public IAssetSet<TKey, TAsset> AssetSet<TKey, TAsset>()
        where TAsset : class, IAsset<TKey>
        => (IAssetSet<TKey, TAsset>)assetSetCache[typeof(TAsset)];
}