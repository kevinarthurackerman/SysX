namespace SysX.JobEngine;

/// <summary>
/// A service used to get the queue which is executing the current job.
/// </summary>
public interface IQueueContext
{
    /// <summary>
    /// The queue executing the current job.
    /// </summary>
    public IQueue Current { get; }

    internal void SetCurrent(IQueue queue);
}
