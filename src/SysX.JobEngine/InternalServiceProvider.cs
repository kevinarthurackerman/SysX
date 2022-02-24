namespace SysX.JobEngine;

/// <summary>
/// An internal service provided used by the engine and the queue.
/// </summary>
public class InternalServiceProvider : IEngineServiceProvider, IQueueServiceProvider
{
    private IServiceProvider? serviceProvider;

    public InternalServiceProvider() { }

    public InternalServiceProvider(IServiceProvider serviceProvider)
    {
        EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));

        this.serviceProvider = serviceProvider;
    }

    public object? GetService(Type serviceType)
    {
        EnsureArg.IsNotNull(serviceType, nameof(serviceType));

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
