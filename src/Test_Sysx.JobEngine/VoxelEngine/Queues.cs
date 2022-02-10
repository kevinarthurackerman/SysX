namespace Test_SysX.JobEngine.VoxelEngine;

public class ConfigQueue : Queue
{
    public ConfigQueue(IQueueServiceProvider queueServiceProvider) : base(queueServiceProvider) { }
}

public class MainQueue : Queue
{
    public MainQueue(IQueueServiceProvider queueServiceProvider) : base(queueServiceProvider) { }
}

public class ContouringQueue : Queue
{
    public ContouringQueue(IQueueServiceProvider queueServiceProvider) : base(queueServiceProvider) { }
}