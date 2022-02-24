namespace SysX.JobEngine;

// TODO: should the job return anything?

/// <summary>
/// Response data for an executing job.
/// </summary>
public readonly record struct OnJobExecuteEventResultData<TJob, TJobExecutor>()
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;
