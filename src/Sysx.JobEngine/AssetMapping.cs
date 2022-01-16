namespace Sysx.JobEngine;

public interface IAssetMapping
{
    public Type AssetKeyType { get; }
    public Type AssetType { get; }
}

public class AssetMapping<TKey, TAsset> : IAssetMapping
    where TAsset : class, IAsset<TKey>
{
    public AssetMapping()
    {
        AssetKeyType = typeof(TKey);
        AssetType = typeof(TAsset);
    }

    public Type AssetKeyType { get; }
    public Type AssetType { get; }
}