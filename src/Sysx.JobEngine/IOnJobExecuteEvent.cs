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

/// <summary>
/// Request data for an executing job.
/// </summary>
public readonly record struct OnJobExecuteEventRequestData<TJob, TJobExecutor>(
    TJob Job,
    TJobExecutor JobExecutor)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

/// <summary>
/// Request data for an executing job.
/// </summary>
public readonly record struct OnJobExecuteEventRequest<TJob, TJobExecutor>(
    in OnJobExecuteEventRequestData<TJob, TJobExecutor> Original,
    in OnJobExecuteEventRequestData<TJob, TJobExecutor> Current)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

// todo: should the job return anything?

/// <summary>
/// Response data for an executing job.
/// </summary>
public readonly record struct OnJobExecuteEventResultData<TJob, TJobExecutor>()
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

/// <summary>
/// Response data for an executing job.
/// </summary>
public readonly record struct OnJobExecuteEventResult<TJob, TJobExecutor>(
    in OnJobExecuteEventResultData<TJob, TJobExecutor> Original,
    in OnJobExecuteEventResultData<TJob, TJobExecutor> Current)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

/// <summary>
/// Calls the next handler in the chain or calls the job itself at the end of the chain.
/// </summary>
public delegate OnJobExecuteEventResult<TJob, TJobExecutor> OnJobExecuteEventNext<TJob, TJobExecutor>(
    in OnJobExecuteEventRequestData<TJob, TJobExecutor> requestData)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;