namespace Sysx.EntityFramework.Plugins;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection UseEntityFrameworkContainerTypes(this IServiceCollection services)
    {
        EnsureArg.IsNotNull(services, nameof(services));

        return services.UpsertSingleton<IMemberTranslatorPlugin, ContainerTypesMemberTranslatorPlugin>()
            .UpsertSingleton<IMethodCallTranslatorPlugin, ContainerTypesMethodCallTranslatorPlugin>()
            .UpsertSingleton<IRelationalTypeMappingSourcePlugin, ContainerTypesRelationalTypeMappingSourcePlugin>();
    }
}