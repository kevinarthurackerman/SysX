namespace Sysx.JobEngine;

/// <summary>
/// An asset that can be read from and manipulated during the execution of a job.
/// </summary>
public interface IAsset<TKey>
{
    public TKey Key { get; }
}
