namespace SysX.JobEngine;

/// <summary>
/// Default service for wrapping all jobs and their wrapping handlers in a transaction.
/// </summary>
public class OnJobExecuteTransactionScope<TJob, TJobExecutor> : IOnJobExecuteEvent<TJob, TJobExecutor>
	where TJob : IJob
	where TJobExecutor : IJobExecutor<TJob>
{
	public OnJobExecuteEventResultData<TJob, TJobExecutor> Execute(
		in OnJobExecuteEventRequest<TJob, TJobExecutor> request, OnJobExecuteEventNext<TJob, TJobExecutor> next)
	{
		EnsureArg.HasValue(request, nameof(request));
		EnsureArg.IsNotNull(next, nameof(next));

		using var transactionScope = new TransactionScope();

		var result = next(request.Current);

		transactionScope.Complete();

		return result.Current;
	}
}
