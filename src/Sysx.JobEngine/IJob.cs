namespace Sysx.JobEngine;

public interface IJob { }

public interface IJobExecutor<TJob>
    where TJob : IJob
{
    public void Execute(in TJob data);
}