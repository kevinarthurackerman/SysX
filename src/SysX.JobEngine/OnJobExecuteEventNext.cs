namespace SysX.JobEngine;

/// <summary>
/// Calls the next handler in the chain or calls the job itself at the end of the chain.
/// </summary>
public delegate OnJobExecuteEventResult<TJob, TJobExecutor> OnJobExecuteEventNext<TJob, TJobExecutor>(
	in OnJobExecuteEventRequestData<TJob, TJobExecutor> requestData)
	where TJob : IJob
	where TJobExecutor : IJobExecutor<TJob>;
