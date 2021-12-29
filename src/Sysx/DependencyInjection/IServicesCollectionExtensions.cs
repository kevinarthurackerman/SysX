namespace Sysx.DependencyInjection;

public static class IServicesCollectionExtensions
{
    public static IServiceCollection UpsertSingleton<TService, TImplementation>(this IServiceCollection services) =>
    services.Upsert(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);

    public static IServiceCollection UpsertScoped<TService, TImplementation>(this IServiceCollection services) =>
        services.Upsert(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);

    public static IServiceCollection UpsertTransient<TService, TImplementation>(this IServiceCollection services) =>
        services.Upsert(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient);

    public static IServiceCollection Upsert<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime) =>
        services.Upsert(typeof(TService), typeof(TImplementation), serviceLifetime);

    public static IServiceCollection UpsertSingleton(this IServiceCollection services, Type serviceType, Type implementationType) =>
        services.Upsert(serviceType, implementationType, ServiceLifetime.Singleton);

    public static IServiceCollection UpsertScoped(this IServiceCollection services, Type serviceType, Type implementationType) =>
        services.Upsert(serviceType, implementationType, ServiceLifetime.Scoped);

    public static IServiceCollection UpsertTransient(this IServiceCollection services, Type serviceType, Type implementationType) =>
        services.Upsert(serviceType, implementationType, ServiceLifetime.Transient);

    public static IServiceCollection Upsert(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
    {
        EnsureArg.IsNotNull(services, nameof(services));
        EnsureArg.IsNotNull(serviceType, nameof(serviceType));
        EnsureArg.IsNotNull(implementationType, nameof(implementationType));
        EnsureArg.IsTrue(
            serviceType.IsAssignableFrom(implementationType),
            nameof(implementationType),
            x => x.WithMessage($"{nameof(implementationType)} {implementationType.FullName} is not assignable to {nameof(serviceType)} {serviceType.FullName}."));

        var existingServices = services
            .Where(x => x.ServiceType == serviceType && x.ImplementationType == implementationType)
            .ToArray();

        foreach (var service in existingServices) services.Remove(service);

        var descriptor = new ServiceDescriptor(serviceType, implementationType, serviceLifetime);

        services.Add(descriptor);

        return services;
    }

    public static IServiceCollection UpsertSingleton<TService>(this IServiceCollection services, Assembly sourceAssembly) =>
        services.Upsert(typeof(TService), sourceAssembly, ServiceLifetime.Singleton);

    public static IServiceCollection UpsertScoped<TService>(this IServiceCollection services, Assembly sourceAssembly) =>
        services.Upsert(typeof(TService), sourceAssembly, ServiceLifetime.Scoped);

    public static IServiceCollection UpsertTransient<TService>(this IServiceCollection services, Assembly sourceAssembly) =>
        services.Upsert(typeof(TService), sourceAssembly, ServiceLifetime.Transient);

    public static IServiceCollection Upsert<TService>(this IServiceCollection services, Assembly sourceAssembly, ServiceLifetime serviceLifetime) =>
        services.Upsert(typeof(TService), sourceAssembly, serviceLifetime);

    public static IServiceCollection UpsertSingleton(this IServiceCollection services, Type serviceType, Assembly sourceAssembly) =>
        services.Upsert(serviceType, sourceAssembly, ServiceLifetime.Singleton);

    public static IServiceCollection UpsertScoped(this IServiceCollection services, Type serviceType, Assembly sourceAssembly) =>
        services.Upsert(serviceType, sourceAssembly, ServiceLifetime.Scoped);

    public static IServiceCollection UpsertTransient(this IServiceCollection services, Type serviceType, Assembly sourceAssembly) =>
        services.Upsert(serviceType, sourceAssembly, ServiceLifetime.Transient);

    public static IServiceCollection Upsert(this IServiceCollection services, Type serviceType, Assembly sourceAssembly, ServiceLifetime serviceLifetime)
    {
        EnsureArg.IsNotNull(services, nameof(services));
        EnsureArg.IsNotNull(serviceType, nameof(serviceType));
        EnsureArg.IsNotNull(sourceAssembly, nameof(sourceAssembly));

        var implementationTypes = sourceAssembly.GetTypes()
            .Where(x => serviceType.IsAssignableFrom(x))
            .ToArray();

        foreach (var implementationType in implementationTypes)
            services.Upsert(serviceType, implementationType!, serviceLifetime);

        return services;
    }
}