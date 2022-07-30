namespace SysX.JobEngine;

public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds an asset context to the service collection.
	/// Asset contexts should only be added to the queue services.
	/// </summary>
	public static IServiceCollection AddAssetContext(
		this IServiceCollection services,
		Type assetContextType,
		IEnumerable<Type>? assetTypes = null,
		ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
	{
		EnsureArg.HasValue(services, nameof(services));
		EnsureArg.HasValue(assetContextType, nameof(assetContextType));

		var isAssetContextType = typeof(AssetContext).IsAssignableFrom(assetContextType);

		if (!isAssetContextType)
			throw new InvalidOperationException($"Type {nameof(isAssetContextType)} {isAssetContextType} is not an AssetContext type.");

		services.Add(new ServiceDescriptor(assetContextType, services => services.Activate(assetContextType, assetTypes ?? Type.EmptyTypes), serviceLifetime));

		var ancestorContextType = assetContextType.BaseType;

		while (ancestorContextType != null)
		{
			if (!ancestorContextType.IsAbstract && ancestorContextType.IsPublic)
			{
				services.Add(new ServiceDescriptor(ancestorContextType, services => services.GetRequiredService(assetContextType), serviceLifetime));
			}

			ancestorContextType = ancestorContextType.BaseType;
		}

		return services;
	}

	/// <summary>
	/// Adds a queue type as a service to a queue.
	/// Queue types should only be added to the queue services.
	/// The queue must be created by the engine before it can be referenced by a job.
	/// </summary>
	public static IServiceCollection AddQueueServiceToQueue(
		this IServiceCollection services,
		Type queueType,
		string name = QueueLocator.DefaultQueueName,
		ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
	{
		EnsureArg.HasValue(services, nameof(services));
		EnsureArg.HasValue(queueType, nameof(queueType));

		var isQueueType = typeof(IQueue).IsAssignableFrom(queueType);

		if (!isQueueType)
			throw new InvalidOperationException($"Type {nameof(isQueueType)} {isQueueType} is not a Queue type.");

		services.Add(new ServiceDescriptor(
			queueType,
			serviceProvider => serviceProvider.GetRequiredService<IQueueLocator>().Get(queueType, name),
			serviceLifetime));

		return services;
	}

	/// <summary>
	/// Adds a job executor to the service collection.
	/// Job executors should only be added to the queue services.
	/// </summary>
	public static IServiceCollection AddJobExecutor(
		this IServiceCollection services,
		Type jobExecutorType,
		ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
	{
		EnsureArg.HasValue(services, nameof(services));
		EnsureArg.HasValue(jobExecutorType, nameof(jobExecutorType));

		var isJobExecutor = jobExecutorType.IsAssignableToGenericType(typeof(IJobExecutor<>));

		if (!isJobExecutor)
			throw new InvalidOperationException($"Type {nameof(jobExecutorType)} {jobExecutorType} is not a JobExecutor type.");

		services.AddClosedType(typeof(IJobExecutor<>), jobExecutorType, serviceLifetime);

		return services;
	}

	/// <summary>
	/// Adds an on job execute event to the service collection.
	/// On job execute events should only be added to the queue services.
	/// </summary>
	public static IServiceCollection AddOnJobExecute(
		this IServiceCollection services,
		Type onJobExecuteType,
		ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
	{
		EnsureArg.HasValue(services, nameof(services));
		EnsureArg.HasValue(onJobExecuteType, nameof(onJobExecuteType));

		var isOnJobExecute = onJobExecuteType.IsAssignableToGenericType(typeof(IOnJobExecuteEvent<,>));

		if (!isOnJobExecute)
			throw new InvalidOperationException($"Type {nameof(onJobExecuteType)} {onJobExecuteType} is not an OnJobExecute type.");

		services.AddOpenOrClosedType(typeof(IOnJobExecuteEvent<,>), onJobExecuteType, serviceLifetime);

		return services;
	}

	private static IServiceCollection AddClosedType(
		this IServiceCollection services,
		Type openServiceType,
		Type implementationType,
		ServiceLifetime serviceLifetime)
	{
		var implementedOpenServiceType = implementationType.GetGenericTypeImplementation(openServiceType);

		EnsureArg.IsNotNull(
			implementedOpenServiceType,
			optsFn: x => x.WithMessage($"Type {implementationType} must implemenet {openServiceType}."));
		EnsureArg.IsFalse(
			implementedOpenServiceType.ContainsGenericParameters,
			optsFn: x => x.WithMessage($"Type {implementationType} is an open type and must be closed."));

		if (implementedOpenServiceType.ContainsGenericParameters)
		{
			services.Add(new ServiceDescriptor(openServiceType, implementationType, serviceLifetime));
		}
		else
		{
			services.Add(new ServiceDescriptor(implementedOpenServiceType, implementationType, serviceLifetime));
		}

		return services;
	}

	private static IServiceCollection AddOpenOrClosedType(
		this IServiceCollection services,
		Type openServiceType,
		Type implementationType,
		ServiceLifetime serviceLifetime)
	{
		var implementedOpenServiceType = implementationType.GetGenericTypeImplementation(openServiceType);

		EnsureArg.IsNotNull(implementedOpenServiceType, optsFn: x => x.WithMessage($"Type {implementationType} must implemenet {openServiceType}."));

		if (implementedOpenServiceType.ContainsGenericParameters)
		{
			services.Add(new ServiceDescriptor(openServiceType, implementationType, serviceLifetime));
		}
		else
		{
			services.Add(new ServiceDescriptor(implementedOpenServiceType, implementationType, serviceLifetime));
		}

		return services;
	}
}
