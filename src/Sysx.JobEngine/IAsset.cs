namespace Sysx.JobEngine;

public interface IAsset<TKey>
{
    public TKey Key { get; }
}
