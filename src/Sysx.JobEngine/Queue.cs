namespace Sysx.JobEngine;

public interface IQueue
{
    public void SubmitJob<TJob>(TJob data)
        where TJob : IJob;
}

public class Queue : IQueue, ISinglePhaseNotification, IAsyncDisposable
{
    private readonly IQueueServiceProvider queueServiceProvider;
    private readonly BlockingCollection<IJobRunner> coordinatingQueue;
    private readonly List<IJobRunner> transactionCoordinatingQueue;
    private readonly Dictionary<Type, IJobRunner> jobRunners;
    private Transaction? transaction;
    private bool disposed;
    private readonly TaskCompletionSource<object> coordinatingQueueCompletionSource;
    private List<Exception>? backgroundExceptions;

    public Queue(IQueueServiceProvider queueServiceProvider)
    {
        this.queueServiceProvider = queueServiceProvider;
        coordinatingQueue = new();
        transactionCoordinatingQueue = new();
        jobRunners = new();
        disposed = false;
        coordinatingQueueCompletionSource = new();
        backgroundExceptions = null;

        new Thread(RunJobs) { Name = GetType().Name }.Start();
    }

    public void SubmitJob<TJob>(TJob data)
        where TJob : IJob
    {
        Ensure.That(this).IsNotAsyncDisposed(disposed);

        ThrowBackgroundExceptionIfAny();

        EnlistTransaction();

        if (!jobRunners.TryGetValue(typeof(TJob), out var jobRunner))
        {
            jobRunner = new JobRunner<TJob>(queueServiceProvider);
            jobRunners[typeof(TJob)] = jobRunner;
        }

        ((JobRunner<TJob>)jobRunner).AddJob(data);

        if(transaction == null)
        {
            coordinatingQueue.Add(jobRunner);
        }
        else
        {
            transactionCoordinatingQueue.Add(jobRunner);
        }
    }

    public async ValueTask DisposeAsync()
    {
        EnsureArg.IsTrue(transaction == null, optsFn: x => x.WithMessage("Cannot dispose while enlisted in a transaction."));

        ThrowBackgroundExceptionIfAny();

        if (disposed)
        {
            await coordinatingQueueCompletionSource.Task;
            return;
        }

        disposed = true;

        coordinatingQueue.CompleteAdding();

        await coordinatingQueueCompletionSource.Task;

        coordinatingQueue.Dispose();

        ThrowBackgroundExceptionIfAny();
    }

    private void RunJobs()
    {
        IJobRunner? nextJobRunner = null;
        try
        {
            foreach (var jobRunner in coordinatingQueue.GetConsumingEnumerable())
            {
                nextJobRunner = jobRunner;
                jobRunner.RunNext();
                nextJobRunner = null;
            }
        }
        catch (Exception ex)
        {
            if (backgroundExceptions == null)
                backgroundExceptions = new List<Exception>();
            

            if (nextJobRunner == null)
            {
                backgroundExceptions.Add(new AggregateException($"Exception thrown before job type could be resolved. See inner exceptions for details.", ex));
            }
            else
            {
                backgroundExceptions.Add(new AggregateException($"Exception throw while running job type {nextJobRunner.GetJobType()}. See inner exceptions for details.", ex));
            }
        }
        finally
        {
            coordinatingQueueCompletionSource.SetResult(new { });
        }
    }

    private void ThrowBackgroundExceptionIfAny()
    {
        if (backgroundExceptions != null)
        {
            throw new AggregateException("One or more exceptions were throw while running jobs in the background. See inner exceptions for details.", backgroundExceptions);
        }
    }

    void ISinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
    {
        Commit();
        singlePhaseEnlistment.Committed();
    }

    void IEnlistmentNotification.Commit(Enlistment enlistment)
    {
        Commit();
        enlistment.Done();
    }

    void IEnlistmentNotification.InDoubt(Enlistment enlistment)
    {
        enlistment.Done();
    }

    void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
    {
        preparingEnlistment.Prepared();
    }

    void IEnlistmentNotification.Rollback(Enlistment enlistment)
    {
        transaction = null;

        foreach (var jobRunner in jobRunners.Values)
            jobRunner.Rollback();

        transactionCoordinatingQueue.Clear();

        enlistment.Done();
    }

    private void EnlistTransaction()
    {
        if (Transaction.Current != null && Transaction.Current != transaction)
        {
            transaction = Transaction.Current;
            transaction.EnlistVolatile(this, EnlistmentOptions.None);
        }
    }

    private void Commit()
    {
        transaction = null;

        foreach (var jobRunner in jobRunners.Values)
            jobRunner.Commit();

        foreach(var jobRunner in transactionCoordinatingQueue)
            coordinatingQueue.Add(jobRunner);

        transactionCoordinatingQueue.Clear();
    }

    private interface IJobRunner
    {
        internal Type GetJobType();
        internal void RunNext();
        internal void Commit();
        internal void Rollback();
    }

    private class JobRunner<TJob> : IJobRunner
        where TJob : IJob
    {
        private readonly LinkedList<TJob> jobs = new();
        private readonly List<TJob> transactionJobs = new();
        private readonly IQueueServiceProvider queueServiceProvider;
        private readonly object @lock = new { };

        internal JobRunner(IQueueServiceProvider queueServiceProvider)
        {
            this.queueServiceProvider = queueServiceProvider;
        }

        internal void AddJob(TJob job)
        {
            lock (@lock)
            {
                if (Transaction.Current == null)
                {
                    jobs.AddLast(job);
                }
                else
                {
                    transactionJobs.Add(job);
                }
            }
        }

        Type IJobRunner.GetJobType() => typeof(TJob);

        void IJobRunner.Commit()
        {
            lock (@lock)
            {
                foreach (var job in transactionJobs)
                    jobs.AddLast(job);

                transactionJobs.Clear();
            }
        }

        void IJobRunner.Rollback()
        {
            lock (@lock)
            {
                transactionJobs.Clear();
            }
        }

        void IJobRunner.RunNext()
        {
            TJob? job;
            lock (@lock)
            {
                job = jobs.First();
                jobs.RemoveFirst();
            }

            using var jobScope = queueServiceProvider.CreateScope();

            var executor = jobScope.ServiceProvider.GetRequiredService<IJobExecutor<TJob>>();

            var initialRequestData = new OnJobExecuteEventRequestData<TJob, IJobExecutor<TJob>>(job, executor);

            var handler = RootHandler;

            var eventHandlers = queueServiceProvider
                .GetServices<IOnJobExecuteEvent<TJob, IJobExecutor<TJob>>>()
                .Reverse()
                .ToArray();

            OnJobExecuteEventResultData<TJob, IJobExecutor<TJob>>? previousResultData = null;
            OnJobExecuteEventResultData<TJob, IJobExecutor<TJob>>? currentResultData = null;

            foreach (var eventHandler in eventHandlers)
            {
                var innerHandler = handler;
                handler = (in OnJobExecuteEventRequestData<TJob, IJobExecutor<TJob>> currentRequestData) =>
                {
                    var request = new OnJobExecuteEventRequest<TJob, IJobExecutor<TJob>>(in initialRequestData, in currentRequestData);
                    currentResultData = eventHandler.Execute(in request, (in OnJobExecuteEventRequestData<TJob, IJobExecutor<TJob>> requestData) => innerHandler(requestData));
                    return new OnJobExecuteEventResult<TJob, IJobExecutor<TJob>>(previousResultData!.Value, currentResultData.Value);
                };
            }

            var handlerResult = handler(initialRequestData);

            OnJobExecuteEventResult<TJob, IJobExecutor<TJob>> RootHandler(in OnJobExecuteEventRequestData<TJob, IJobExecutor<TJob>> requestData)
            {
                requestData.JobExecutor.Execute(requestData.Job);

                previousResultData = new OnJobExecuteEventResultData<TJob, IJobExecutor<TJob>>();
                var currentResultData = new OnJobExecuteEventResultData<TJob, IJobExecutor<TJob>>();
                return new OnJobExecuteEventResult<TJob, IJobExecutor<TJob>>(previousResultData.Value, currentResultData);
            }
        }
    }
}
