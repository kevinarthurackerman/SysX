namespace Sysx.JobEngine;

public interface IQueue
{
    public void SubmitJob<TJob>(TJob data)
        where TJob : IJob;

    public void SubmitChildJob<TJob>(TJob jobData)
        where TJob : IJob;
}

public class Queue : IQueue, IDisposable
{
    private static int queueInstanceNumber = 0;

    private readonly IQueueServiceProvider queueServiceProvider;
    private readonly Dictionary<Type, object> jobRunnerObjectPools = new();
    private readonly List<IJobRunner> jobRunnersToRun = new();
    private readonly Dictionary<Transaction, TransactionJobRunners> transactionJobRunners = new();
    private readonly Action<Transaction> rollback;
    private readonly Action<Transaction, TransactionJobRunners> commit;
    private List<Exception>? backgroundExceptions;
    private bool disposed = false;
    private readonly object jobRunnersUpdateLock = new { };

    public Queue(IQueueServiceProvider queueServiceProvider)
    {
        this.queueServiceProvider = queueServiceProvider;

        rollback = transaction => Rollback(transaction);
        commit = (transaction, runners) => Commit(transaction, runners);

        new Thread(RunJobs) { Name = $"{nameof(Queue)} {queueInstanceNumber++} '{GetType().Name}'" }.Start();
    }

    public void SubmitJob<TJob>(TJob jobData)
        where TJob : IJob
    {
        lock (jobRunnersUpdateLock)
        {
            Ensure.That(this).IsNotDisposed(disposed);

            ThrowBackgroundExceptionIfAny();

            var jobRunner = CreateJobRunner(jobData);

            if (Transaction.Current == null)
            {
                jobRunnersToRun.Add(jobRunner);

                JobRunnersUpdated();
            }
            else
            {
                var jobRunners = GetTransactionJobRunners(Transaction.Current);
                jobRunners.Jobs.Add(jobRunner);
            }
        }
    }

    public void SubmitChildJob<TJob>(TJob jobData)
        where TJob : IJob
    {
        lock (jobRunnersUpdateLock)
        {
            Ensure.That(this).IsNotDisposed(disposed);
            EnsureArg.IsNotNull(Transaction.Current, optsFn: x => x.WithMessage($"{nameof(SubmitChildJob)} must be executed within the scope of a {nameof(Transaction)}."));

            ThrowBackgroundExceptionIfAny();

            var jobRunner = CreateJobRunner(jobData);

            var jobRunners = GetTransactionJobRunners(Transaction.Current);
            jobRunners.ChildJobs.Add(jobRunner);
        }
    }

    public void Dispose()
    {
        ThrowBackgroundExceptionIfAny();

        if (disposed) return;

        disposed = true;

        while (true)
        {
            try
            {
                Monitor.Enter(jobRunnersUpdateLock);

                if (!jobRunnersToRun.Any())
                {
                    ThrowBackgroundExceptionIfAny();

                    return;
                }
                else
                {
                    Monitor.Wait(jobRunnersUpdateLock);
                }
            }
            finally
            { 
                Monitor.Exit(jobRunnersUpdateLock);
            }
        }
    }

    private JobRunner<TJob> CreateJobRunner<TJob>(TJob jobData)
        where TJob : IJob
    {
        if (!jobRunnerObjectPools.TryGetValue(typeof(TJob), out var jobRunnerObjectPool))
        {
            jobRunnerObjectPool = ObjectPool.Create<JobRunner<TJob>>();
            jobRunnerObjectPools[typeof(TJob)] = jobRunnerObjectPool;
        }

        var jobRunner = ((ObjectPool<JobRunner<TJob>>)jobRunnerObjectPool).Get();
        jobRunner.Prepare(jobData, queueServiceProvider);

        return jobRunner;
    }

    private TransactionJobRunners GetTransactionJobRunners(Transaction transaction)
    {
        if (!transactionJobRunners.TryGetValue(transaction, out var jobRunners))
        {
            jobRunners = new TransactionJobRunners(transaction, commit, rollback);
            transactionJobRunners[transaction] = jobRunners;
        }

        return jobRunners;
    }

    private void ThrowBackgroundExceptionIfAny()
    {
        if (backgroundExceptions != null)
        {
            throw new AggregateException("One or more exceptions were throw while running jobs in the background. See inner exceptions for details.", backgroundExceptions);
        }
    }

    private void RunJobs()
    {
        while (true)
        {
            try
            {
                IJobRunner? nextJobRunner = null;

                try
                {
                    Monitor.Enter(jobRunnersUpdateLock);

                    if (!jobRunnersToRun.Any())
                    {
                        if (disposed && !transactionJobRunners.Any()) return;

                        Monitor.Wait(jobRunnersUpdateLock);
                    }

                    nextJobRunner = jobRunnersToRun.First();
                    jobRunnersToRun.RemoveAt(0);
                    JobRunnersUpdated();
                }
                finally
                {
                    Monitor.Exit(jobRunnersUpdateLock);
                }

                nextJobRunner.Execute();
            }
            catch (Exception ex)
            {
                if (backgroundExceptions == null)
                    backgroundExceptions = new List<Exception>();

                backgroundExceptions.Add(ex);
            }
        }
    }

    private void Rollback(Transaction transaction)
    {
        lock (jobRunnersUpdateLock)
        {
            transactionJobRunners.Remove(transaction);
        }
    }

    private void Commit(Transaction transaction, TransactionJobRunners transactionJobRunners)
    {
        lock (jobRunnersUpdateLock)
        {
            jobRunnersToRun.InsertRange(0, transactionJobRunners.ChildJobs);
            jobRunnersToRun.AddRange(transactionJobRunners.Jobs);
            this.transactionJobRunners.Remove(transaction);

            JobRunnersUpdated();
        }
    }

    private void JobRunnersUpdated() => Monitor.PulseAll(jobRunnersUpdateLock);

    private interface IJobRunner
    {
        internal Type JobType { get; }
        internal void Execute();
    }

    private class JobRunner<TJob> : IJobRunner
        where TJob : IJob
    {
        private TJob? jobData;
        private IQueueServiceProvider? queueServiceProvider;

        Type IJobRunner.JobType { get; } = typeof(TJob);

        internal void Prepare(TJob jobData, IQueueServiceProvider queueServiceProvider)
        {
            this.jobData = jobData;
            this.queueServiceProvider = queueServiceProvider;
        }

        void IJobRunner.Execute()
        {
            EnsureArg.HasValue(jobData, nameof(jobData));
            EnsureArg.IsNotNull(queueServiceProvider, nameof(queueServiceProvider));

            using var jobScope = queueServiceProvider.CreateScope();

            var executor = jobScope.ServiceProvider.GetRequiredService<IJobExecutor<TJob>>();

            var initialRequestData = new OnJobExecuteEventRequestData<TJob, IJobExecutor<TJob>>(jobData, executor);

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
                    try
                    {
                        currentResultData = eventHandler.Execute(in request, (in OnJobExecuteEventRequestData<TJob, IJobExecutor<TJob>> requestData) => innerHandler(requestData));
                    }
                    catch (Exception ex)
                    {
                        if (ex is OnJobExecuteEventException || ex is JobExecuteException) throw;

                        throw new OnJobExecuteEventException($"An exception was thrown calling {nameof(eventHandler.Execute)} on {eventHandler.GetType()}. See inner exception for details.", ex);
                    }

                    return new OnJobExecuteEventResult<TJob, IJobExecutor<TJob>>(previousResultData!.Value, currentResultData.Value);
                };
            }

            var handlerResult = handler(initialRequestData);

            jobData = default;
            queueServiceProvider = null;

            OnJobExecuteEventResult<TJob, IJobExecutor<TJob>> RootHandler(in OnJobExecuteEventRequestData<TJob, IJobExecutor<TJob>> requestData)
            {
                try
                {
                    requestData.JobExecutor.Execute(requestData.Job);

                    previousResultData = new OnJobExecuteEventResultData<TJob, IJobExecutor<TJob>>();
                    var currentResultData = new OnJobExecuteEventResultData<TJob, IJobExecutor<TJob>>();
                    return new OnJobExecuteEventResult<TJob, IJobExecutor<TJob>>(previousResultData.Value, currentResultData);
                }
                catch (Exception ex)
                {
                    throw new JobExecuteException($"An exception was thrown calling {nameof(IJobRunner.Execute)} on {executor.GetType()}. See inner exception for details.", ex);
                }
            }
        }
    }

    private class TransactionJobRunners : ISinglePhaseNotification
    {
        private readonly Transaction transaction;
        private readonly Action<Transaction, TransactionJobRunners> commit;
        private readonly Action<Transaction> rollback;

        internal List<IJobRunner> Jobs = new();
        internal List<IJobRunner> ChildJobs = new();

        internal TransactionJobRunners(Transaction transaction, Action<Transaction, TransactionJobRunners> commit, Action<Transaction> rollback)
        {
            this.transaction = transaction;
            this.commit = commit;
            this.rollback = rollback;

            transaction.EnlistVolatile(this, EnlistmentOptions.None);
        }

        void ISinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
        {
            commit(transaction, this);
            singlePhaseEnlistment.Committed();
        }

        void IEnlistmentNotification.Commit(Enlistment enlistment)
        {
            commit(transaction, this);
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
            rollback(transaction);
            enlistment.Done();
        }
    }
}
