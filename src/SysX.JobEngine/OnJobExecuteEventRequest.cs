namespace SysX.JobEngine;

/// <summary>
/// Request data for an executing job.
/// </summary>
public readonly record struct OnJobExecuteEventRequest<TJob, TJobExecutor>(
    in OnJobExecuteEventRequestData<TJob, TJobExecutor> Original,
    in OnJobExecuteEventRequestData<TJob, TJobExecutor> Current)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;
