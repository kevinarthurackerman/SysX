namespace SysX.JobEngine;

/// <summary>
/// Used to access services provided by the engine.
/// </summary>
public interface IEngineServiceProvider : IServiceProvider
{
    internal void SetServiceProvider(IServiceProvider serviceProvider);
}
