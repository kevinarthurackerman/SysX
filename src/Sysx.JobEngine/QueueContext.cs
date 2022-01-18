namespace Sysx.JobEngine;

public interface IQueueContext
{
    public IQueue Current { get; }

    internal void SetCurrent(IQueue queue);
}

public class QueueContext : IQueueContext
{
    public IQueue Current { get; private set; } = null!;

    void IQueueContext.SetCurrent(IQueue queue)
    {
        Current = queue;
    }
}
