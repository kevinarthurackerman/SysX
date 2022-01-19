namespace Sysx.JobEngine;

public abstract class AssetContext
{
    private readonly Dictionary<Type, IAssetSet> assetSetCache;

    public AssetContext(IEnumerable<Type> assetTypes, IQueueServiceProvider queueServiceProvider)
    {
        assetSetCache = assetTypes.ToDictionary(assetType => assetType, assetType => CreateAssetSet(assetType));

        foreach (var property in GetType().GetProperties())
        {
            var iassetSet = property.PropertyType
                .GetGenericTypeImplementation(typeof(IAssetSet<,>));

            if (iassetSet == null) continue;

            var assetType = iassetSet.GenericTypeArguments[1];

            if (!assetSetCache.TryGetValue(assetType, out var assetSet))
                assetSetCache[assetType] = assetSet = CreateAssetSet(assetType);

            property.SetValue(this, assetSetCache[assetType]);
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
                .Invoke(new object[] { queueServiceProvider });
        }
    }

    public IAssetSet<TKey, TAsset> AssetSet<TKey, TAsset>()
        where TAsset : class, IAsset<TKey>
        => (IAssetSet<TKey, TAsset>)assetSetCache[typeof(TAsset)];
}