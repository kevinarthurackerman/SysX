namespace SysX.JobEngine;

/// <summary>
/// A handler for a job that manipulates assets.
/// </summary>
public interface IJobExecutor<TJob>
    where TJob : IJob
{
    public void Execute(in TJob data);
}
