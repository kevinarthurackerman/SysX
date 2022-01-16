namespace Sysx.JobEngine;

public class OnJobExecute_TransactionScope<TJob, TJobExecutor> : IOnJobExecute<TJob, TJobExecutor>
    where TJob : IJob
    where TJobExecutor : IJobExecutor<TJob>
{
    public OnJobExecuteResultData<TJob, TJobExecutor> Execute(in OnJobExecuteRequest<TJob, TJobExecutor> request, OnJobExecuteNext<TJob, TJobExecutor> next)
    {
        using var transactionScope = new TransactionScope();

        var result = next(request.Current);

        transactionScope.Complete();

        return result.Current;
    }
}
