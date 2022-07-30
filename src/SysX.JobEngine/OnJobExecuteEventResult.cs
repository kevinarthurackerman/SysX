namespace SysX.JobEngine;

/// <summary>
/// Response data for an executing job.
/// </summary>
public readonly record struct OnJobExecuteEventResult<TJob, TJobExecutor>(
	in OnJobExecuteEventResultData<TJob, TJobExecutor> Original,
	in OnJobExecuteEventResultData<TJob, TJobExecutor> Current)
	where TJob : IJob
	where TJobExecutor : IJobExecutor<TJob>;
