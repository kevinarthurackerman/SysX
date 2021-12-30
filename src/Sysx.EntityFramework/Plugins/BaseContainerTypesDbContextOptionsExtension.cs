namespace Sysx.EntityFramework.Plugins;

/// <summary>
/// A base class for implementing IDbContextOptionsExtensions that register types from the services provider.
/// </summary>
public abstract class BaseContainerTypesDbContextOptionsExtension : IDbContextOptionsExtension
{
    /// <summary>
    /// A name for this extension to use for it's Info.
    /// </summary>
    public string Name { get; }

    public virtual DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

    public BaseContainerTypesDbContextOptionsExtension(string extensionName)
    {
        Name = extensionName;
    }

    public virtual void ApplyServices(IServiceCollection services)
    {
        EnsureArg.IsNotNull(services, nameof(services));

        services.UseEntityFrameworkContainerTypes();
    }

    public virtual void Validate(IDbContextOptions options)
    {
        EnsureArg.IsNotNull(options, nameof(options));

        var internalServiceProvider = options.FindExtension<CoreOptionsExtension>()?.InternalServiceProvider;
        if (internalServiceProvider != null)
        {
            using var scope = internalServiceProvider.CreateScope();

            EnsureServiceRegistered<IMemberTranslatorPlugin, ContainerTypesMemberTranslatorPlugin>(scope);
            EnsureServiceRegistered<IMethodCallTranslatorPlugin, ContainerTypesMethodCallTranslatorPlugin>(scope);
            EnsureServiceRegistered<IRelationalTypeMappingSourcePlugin, ContainerTypesRelationalTypeMappingSourcePlugin>(scope);
        }
    }

    protected static void EnsureServiceRegistered<TService, TImplementation>(IServiceScope serviceScope)
    {
        if (!serviceScope.ServiceProvider.GetService<IEnumerable<TService>>()?.Any(x => x is TImplementation) ?? false)
        {
            throw new InvalidOperationException($"Missing required service {nameof(TService)} with implementation {nameof(TImplementation)}");
        }
    }

    private class ExtensionInfo : DbContextOptionsExtensionInfo
    {
        private readonly BaseContainerTypesDbContextOptionsExtension extension;

        public ExtensionInfo(BaseContainerTypesDbContextOptionsExtension extension) : base(extension)
        {
            Assert.That(extension != null);

            this.extension = extension!;
            LogFragment = $"'ContainerTypes:{this.extension.Name}'=Enabled ";
        }

        public override bool IsDatabaseProvider => false;

        public override string LogFragment { get; }

        public override long GetServiceProviderHashCode() => 0;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            EnsureArg.IsNotNull(debugInfo, nameof(debugInfo));

            debugInfo[$"ContainerTypes:{extension.Name}"] = "1";
        }
    }
}