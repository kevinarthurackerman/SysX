namespace Sysx.JobEngine;

public interface IEngineServiceProvider : IServiceProvider { }

public interface IQueueServiceProvider : IServiceProvider { }

public class InternalServiceProvider : IEngineServiceProvider, IQueueServiceProvider
{
    private IServiceProvider? serviceProvider;

    public object? GetService(Type serviceType)
    {
        if (serviceProvider == null)
            throw new InvalidOperationException($"Internal service provider must first be initialized by calling {nameof(SetServiceProvider)} before being accessed.");

        return serviceProvider.GetService(serviceType);
    }

    internal void SetServiceProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
}
