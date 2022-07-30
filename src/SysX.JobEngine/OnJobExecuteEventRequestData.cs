namespace SysX.JobEngine;

/// <summary>
/// Request data for an executing job.
/// </summary>
public readonly record struct OnJobExecuteEventRequestData<TJob, TJobExecutor>(
	TJob Job,
	TJobExecutor JobExecutor)
	where TJob : IJob
	where TJobExecutor : IJobExecutor<TJob>;
