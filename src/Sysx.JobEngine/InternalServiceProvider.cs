namespace Sysx.JobEngine;

public interface IEngineServiceProvider : IServiceProvider
{
    internal void SetServiceProvider(IServiceProvider serviceProvider);
}

public interface IQueueServiceProvider : IServiceProvider
{
    internal void SetServiceProvider(IServiceProvider serviceProvider);
}

public class InternalServiceProvider : IEngineServiceProvider, IQueueServiceProvider
{
    private IServiceProvider? serviceProvider;

    public object? GetService(Type serviceType)
    {
        if (serviceProvider == null)
            throw new InvalidOperationException($"Internal service provider must first be initialized by calling SetServiceProvider before being accessed.");

        return serviceProvider.GetService(serviceType);
    }

    void IQueueServiceProvider.SetServiceProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    void IEngineServiceProvider.SetServiceProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
}
