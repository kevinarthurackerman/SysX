namespace Sysx.JobEngine;

public interface IQueue
{
    void SubmitJob<TJob>(TJob data)
        where TJob : IJob;
}

public class Queue : IQueue
{
    private readonly IServiceProvider queueServiceProvider;

    public Queue(IQueueServiceProvider queueServiceProvider)
    {
        this.queueServiceProvider = queueServiceProvider;
    }

    public void SubmitJob<TJob>(TJob data)
        where TJob : IJob
    {
        using var jobScope = queueServiceProvider.CreateScope();

        var job = jobScope.ServiceProvider.GetRequiredService<IJobExecutor<TJob>>();

        job.Execute(data);
    }
}
