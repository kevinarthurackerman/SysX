namespace Sysx.JobEngine;

public interface IOnJobExecute<TJob, TJobExecutor>
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>
{
    public OnJobExecuteResultData<TJob, TJobExecutor> Execute(in OnJobExecuteRequest<TJob, TJobExecutor> request, OnJobExecuteNext<TJob, TJobExecutor> next);
}

public readonly record struct OnJobExecuteRequestData<TJob, TJobExecutor>(TJob Job, TJobExecutor JobExecutor)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

public readonly record struct OnJobExecuteRequest<TJob, TJobExecutor>(in OnJobExecuteRequestData<TJob, TJobExecutor> Original, in OnJobExecuteRequestData<TJob, TJobExecutor> Current)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

// todo: should the job return anything?
public readonly record struct OnJobExecuteResultData<TJob, TJobExecutor>()
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

public readonly record struct OnJobExecuteResult<TJob, TJobExecutor>(in OnJobExecuteResultData<TJob, TJobExecutor> Original, in OnJobExecuteResultData<TJob, TJobExecutor> Current)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;

public delegate OnJobExecuteResult<TJob, TJobExecutor> OnJobExecuteNext<TJob, TJobExecutor>(in OnJobExecuteRequestData<TJob, TJobExecutor> requestData)
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>;