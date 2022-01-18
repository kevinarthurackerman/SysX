namespace Sysx.JobEngine;

public class OnJobExecute_TransactionScope<TJob, TJobExecutor> : IOnJobExecuteEvent<TJob, TJobExecutor>
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>
{
    public OnJobExecuteEventResultData<TJob, TJobExecutor> Execute(in OnJobExecuteEventRequest<TJob, TJobExecutor> request, OnJobExecuteEventNext<TJob, TJobExecutor> next)
    {
        using var transactionScope = new TransactionScope();

        var result = next(request.Current);

        transactionScope.Complete();

        return result.Current;
    }
}
