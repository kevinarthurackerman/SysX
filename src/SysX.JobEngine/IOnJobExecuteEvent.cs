namespace SysX.JobEngine;

/// <summary>
/// A handler that wraps jobs executing. Calling next calles the next handler in the chain
/// or calls the job itself at the end of the chain.
/// </summary>
public interface IOnJobExecuteEvent<TJob, TJobExecutor>
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>
{
    /// <summary>
    /// Wraps jobs executing. Calling next calles the next handler in the chain
    /// or calls the job itself at the end of the chain.
    /// </summary>
    public OnJobExecuteEventResultData<TJob, TJobExecutor> Execute(
        in OnJobExecuteEventRequest<TJob, TJobExecutor> request,
        OnJobExecuteEventNext<TJob, TJobExecutor> next);
}
