namespace Sysx.JobEngine;

public interface IQueue
{
    public void SubmitJob<TJob>(TJob data)
        where TJob : IJob;
}

public class Queue : IQueue, IAsyncDisposable
{
    private readonly IQueueServiceProvider queueServiceProvider;
    private readonly BlockingCollection<IJobRunner> coordinatingQueue;
    private readonly Dictionary<Type, IJobRunner> jobRunners;
    private bool disposed;
    private TaskCompletionSource<object> coordinatingQueueCompletionSource;
    private List<Exception>? backgroundExceptions;

    public Queue(IQueueServiceProvider queueServiceProvider)
    {
        this.queueServiceProvider = queueServiceProvider;
        coordinatingQueue = new();
        jobRunners = new();
        disposed = false;
        coordinatingQueueCompletionSource = new();
        backgroundExceptions = null;

        new Thread(RunJobs) { Name = GetType().Name }.Start();
    }

    public void SubmitJob<TJob>(TJob data)
        where TJob : IJob
    {
        ThrowBackgroundExceptionIfAny();

        if (!jobRunners.TryGetValue(typeof(TJob), out var jobRunner))
        {
            jobRunner = new JobRunner<TJob>(queueServiceProvider);
            jobRunners[typeof(TJob)] = jobRunner;
        }

        ((JobRunner<TJob>)jobRunner).AddJob(data);

        coordinatingQueue.Add(jobRunner);
    }

    public async ValueTask DisposeAsync()
    {
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
    }

    private void RunJobs()
    {
        try
        {
            foreach (var jobRunner in coordinatingQueue.GetConsumingEnumerable())
            {
                jobRunner.RunNext();
            }
        }
        catch (Exception ex)
        {
            if (backgroundExceptions == null)
                backgroundExceptions = new List<Exception>();

            backgroundExceptions.Add(ex);
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

    private interface IJobRunner
    {
        void RunNext();
    }

    private class JobRunner<TJob> : IJobRunner
        where TJob : IJob
    {
        private readonly Queue<TJob> jobs = new();
        private readonly IQueueServiceProvider queueServiceProvider;

        public JobRunner(IQueueServiceProvider queueServiceProvider)
        {
            this.queueServiceProvider = queueServiceProvider;
        }

        public void AddJob(TJob job) => jobs.Enqueue(job);

        public void RunNext()
        {
            var data = jobs.Dequeue();

            using var jobScope = queueServiceProvider.CreateScope();

            var executor = jobScope.ServiceProvider.GetRequiredService<IJobExecutor<TJob>>();

            executor.Execute(data);
        }
    }
}
