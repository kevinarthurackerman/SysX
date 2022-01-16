namespace Sysx.JobEngine;

public class AssetContext
{
    private readonly Dictionary<Type, object> assetSetCache;

    public AssetContext(
        IEnumerable<IAssetMapping> assetMappings,
        IQueueServiceProvider queueServiceProvider)
    {
        assetSetCache = assetMappings.ToDictionary(
            assetMapping => assetMapping.AssetType,
            assetMapping =>
            {
                return typeof(AssetSet<,>)
                    .MakeGenericType(assetMapping.AssetKeyType, assetMapping.AssetType)
                    .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Single()
                    .Invoke(new object[] { assetMapping, queueServiceProvider });
            });
    }

    public AssetSet<TKey, TAsset> AssetSet<TKey, TAsset>()
        where TAsset : class, IAsset<TKey>
        => (AssetSet<TKey, TAsset>)assetSetCache[typeof(TAsset)];
}