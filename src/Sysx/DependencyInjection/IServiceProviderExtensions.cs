namespace Sysx.DependencyInjection;

public static class IServiceProviderExtensions
{
    /// <inheritdoc cref="Activate(IServiceProvider, Type, object[])"/>
    public static TService Activate<TService>(this IServiceProvider serviceProvider, params object[] withParameters)
        => (TService)Activate(serviceProvider, typeof(TService), withParameters);


    /// <summary>
    /// Activates an instance of the service type using the parameters provided and the service provider.
    /// This method can be used to activate types not registered to the services provider,
    /// or it can be used when registering types with a service factory to provide some parameters
    /// statically while getting others from the service provider.
    /// </summary>
    public static object Activate(this IServiceProvider serviceProvider, Type serviceType, params object[] withParameters)
    {
        EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));
        Ensure.That(withParameters).DoesNotContainNull();

        var eligibleConstructors = new List<(ConstructorInfo Constructor, object[] Parameters)>();

        foreach (var ctor in serviceType.GetConstructors())
        {
            var ctorParamInfos = ctor.GetParameters();

            var ctorParams = new object?[ctorParamInfos.Length];

            for (var i = 0; i < ctorParamInfos.Length; i++)
            {
                var paramType = ctorParamInfos[i].ParameterType;

                ctorParams[i] = withParameters
                    .FirstOrDefault(x => paramType.IsAssignableFrom(x.GetType()))
                    ?? serviceProvider.GetService(paramType);

                if (ctorParams[i] == null) break;
            }

            if (ctorParams.Any() && ctorParams.Last() == null) continue;

#pragma warning disable CS8620 // All values in ctorParams are not null here.
            eligibleConstructors.Add((ctor, ctorParams));
#pragma warning restore CS8620 // All values in ctorParams are not null here.
        }

        if (eligibleConstructors.Count == 0)
            throw new InvalidOperationException($"A suitable constructor for type '{serviceType}' could not be located. Ensure the type is concrete and services are registered or provided via {nameof(withParameters)} for all parameters of a public constructor.");

        if (eligibleConstructors.Count > 1)
            throw new InvalidOperationException($"Multiple constructors accepting all given argument types have been found in type '{serviceType}'. There should only be one applicable constructor.");

        var (constructor, parameters) = eligibleConstructors.Single();

        return constructor.Invoke(parameters);
    }
}
