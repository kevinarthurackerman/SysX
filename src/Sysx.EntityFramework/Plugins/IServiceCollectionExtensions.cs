namespace Sysx.EntityFramework.Plugins;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required for using ContainerTypesDbContextOptionsExtensions
    /// </summary>
    public static IServiceCollection UseEntityFrameworkContainerTypes(this IServiceCollection services)
    {
        EnsureArg.IsNotNull(services, nameof(services));

        return services.UpsertSingleton<IMemberTranslatorPlugin, ContainerTypesMemberTranslatorPlugin>()
            .UpsertSingleton<IMethodCallTranslatorPlugin, ContainerTypesMethodCallTranslatorPlugin>()
            .UpsertSingleton<IRelationalTypeMappingSourcePlugin, ContainerTypesRelationalTypeMappingSourcePlugin>();
    }
}