namespace Sysx.JobEngine;

public interface IOnJobExecuteEvent<TJob, TJobExecutor>
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>
{
    public OnJobExecuteEventResultData<TJob, TJobExecutor> Execute(in OnJobExecuteEventRequest<TJob, TJobExecutor> request, OnJobExecuteEventNext<TJob, TJobExecutor> next);
}

public readonly record struct OnJobExecuteEventRequestData<TJob, TJobExecutor>(TJob Job, TJobExecutor JobExecutor)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

public readonly record struct OnJobExecuteEventRequest<TJob, TJobExecutor>(in OnJobExecuteEventRequestData<TJob, TJobExecutor> Original, in OnJobExecuteEventRequestData<TJob, TJobExecutor> Current)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

// todo: should the job return anything?
public readonly record struct OnJobExecuteEventResultData<TJob, TJobExecutor>()
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

public readonly record struct OnJobExecuteEventResult<TJob, TJobExecutor>(in OnJobExecuteEventResultData<TJob, TJobExecutor> Original, in OnJobExecuteEventResultData<TJob, TJobExecutor> Current)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

public delegate OnJobExecuteEventResult<TJob, TJobExecutor> OnJobExecuteEventNext<TJob, TJobExecutor>(in OnJobExecuteEventRequestData<TJob, TJobExecutor> requestData)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;