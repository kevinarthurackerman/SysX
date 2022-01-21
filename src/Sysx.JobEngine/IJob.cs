namespace Sysx.JobEngine;

/// <summary>
/// Data for a job that manipulates assets.
/// </summary>
public interface IJob { }

/// <summary>
/// A handler for a job that manipulates assets.
/// </summary>
public interface IJobExecutor<TJob>
    where TJob : IJob
{
    public void Execute(in TJob data);
}