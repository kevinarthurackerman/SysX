namespace Test_SysX.JobEngine.VoxelEngine.OnActionHandlers;

public class OnJobExecuteLog<TJob, TJobExecutor> : IOnJobExecuteEvent<TJob, TJobExecutor>
	where TJob : IJob
	where TJobExecutor : IJobExecutor<TJob>
{
	public OnJobExecuteEventResultData<TJob, TJobExecutor> Execute(
		in OnJobExecuteEventRequest<TJob, TJobExecutor> request,
		OnJobExecuteEventNext<TJob, TJobExecutor> next)
	{
		// do some logging

		return next(request.Current).Current;
	}
}
