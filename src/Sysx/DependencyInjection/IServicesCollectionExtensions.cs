namespace Sysx.DependencyInjection;

public static class IServicesCollectionExtensions
{
    /// <summary>
    /// Upserts the service with a singleton lifetime, removing and replacing any existing implementation.
    /// </summary>
    public static IServiceCollection UpsertSingleton<TService, TImplementation>(this IServiceCollection services) =>
        services.Upsert(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);

    /// <summary>
    /// Upserts the service with a scoped lifetime, removing and replacing any existing implementation.
    /// </summary>
    public static IServiceCollection UpsertScoped<TService, TImplementation>(this IServiceCollection services) =>
        services.Upsert(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);

    /// <summary>
    /// Upserts the service with a transient lifetime, removing and replacing any existing implementation.
    /// </summary>
    public static IServiceCollection UpsertTransient<TService, TImplementation>(this IServiceCollection services) =>
        services.Upsert(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient);

    /// <summary>
    /// Upserts the service, removing and replacing any existing implementation.
    /// </summary>
    public static IServiceCollection Upsert<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime) =>
        services.Upsert(typeof(TService), typeof(TImplementation), serviceLifetime);

    /// <summary>
    /// Upserts the service with a singleton lifetime, removing and replacing any existing implementation.
    /// </summary>
    public static IServiceCollection UpsertSingleton(this IServiceCollection services, Type serviceType, Type implementationType) =>
        services.Upsert(serviceType, implementationType, ServiceLifetime.Singleton);

    /// <summary>
    /// Upserts the service with a scoped lifetime, removing and replacing any existing implementation.
    /// </summary>
    public static IServiceCollection UpsertScoped(this IServiceCollection services, Type serviceType, Type implementationType) =>
        services.Upsert(serviceType, implementationType, ServiceLifetime.Scoped);

    /// <summary>
    /// Upserts the service with a transient lifetime, removing and replacing any existing implementation.
    /// </summary>
    public static IServiceCollection UpsertTransient(this IServiceCollection services, Type serviceType, Type implementationType) =>
        services.Upsert(serviceType, implementationType, ServiceLifetime.Transient);

    /// <summary>
    /// Upserts the service, removing and replacing any existing implementation.
    /// </summary>
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

    /// <summary>
    /// Upserts all services from the assembly that are assignable to the service type with a singleton lifetime and removes any previously existing implementations.
    /// </summary>
    public static IServiceCollection UpsertSingleton<TService>(this IServiceCollection services, Assembly sourceAssembly) =>
        services.Upsert(typeof(TService), sourceAssembly, ServiceLifetime.Singleton);

    /// <summary>
    /// Upserts all services from the assembly that are assignable to the service type with a scoped lifetime and removes any previously existing implementations.
    /// </summary>
    public static IServiceCollection UpsertScoped<TService>(this IServiceCollection services, Assembly sourceAssembly) =>
        services.Upsert(typeof(TService), sourceAssembly, ServiceLifetime.Scoped);

    /// <summary>
    /// Upserts all services from the assembly that are assignable to the service type with a transient lifetime and removes any previously existing implementations.
    /// </summary>
    public static IServiceCollection UpsertTransient<TService>(this IServiceCollection services, Assembly sourceAssembly) =>
        services.Upsert(typeof(TService), sourceAssembly, ServiceLifetime.Transient);

    /// <summary>
    /// Upserts all services from the assembly that are assignable to the service type and removes any previously existing implementations.
    /// </summary>
    public static IServiceCollection Upsert<TService>(this IServiceCollection services, Assembly sourceAssembly, ServiceLifetime serviceLifetime) =>
        services.Upsert(typeof(TService), sourceAssembly, serviceLifetime);

    /// <summary>
    /// Upserts all services from the assembly that are assignable to the service type with a singleton lifetime and removes any previously existing implementations.
    /// </summary>
    public static IServiceCollection UpsertSingleton(this IServiceCollection services, Type serviceType, Assembly sourceAssembly) =>
        services.Upsert(serviceType, sourceAssembly, ServiceLifetime.Singleton);

    /// <summary>
    /// Upserts all services from the assembly that are assignable to the service type with a scoped lifetime and removes any previously existing implementations.
    /// </summary>
    public static IServiceCollection UpsertScoped(this IServiceCollection services, Type serviceType, Assembly sourceAssembly) =>
        services.Upsert(serviceType, sourceAssembly, ServiceLifetime.Scoped);

    /// <summary>
    /// Upserts all services from the assembly that are assignable to the service type with a transient lifetime and removes any previously existing implementations.
    /// </summary>
    public static IServiceCollection UpsertTransient(this IServiceCollection services, Type serviceType, Assembly sourceAssembly) =>
        services.Upsert(serviceType, sourceAssembly, ServiceLifetime.Transient);

    /// <summary>
    /// Upserts all services from the assembly that are assignable to the service type and removes any previously existing implementations.
    /// </summary>
    public static IServiceCollection Upsert(this IServiceCollection services, Type serviceType, Assembly sourceAssembly, ServiceLifetime serviceLifetime)
    {
        EnsureArg.IsNotNull(services, nameof(services));
        EnsureArg.IsNotNull(serviceType, nameof(serviceType));
        EnsureArg.IsNotNull(sourceAssembly, nameof(sourceAssembly));

        var implementationTypes = sourceAssembly.GetTypes()
            .Where(x => (x.IsClass || x.IsValueType) && !x.IsAbstract && serviceType.IsAssignableFrom(x))
            .ToArray();

        foreach (var implementationType in implementationTypes)
            services.Upsert(serviceType, implementationType!, serviceLifetime);

        return services;
    }
}