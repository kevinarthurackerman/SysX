namespace Sysx.DependencyInjection;

public static class IServiceProviderExtensions
{
    public static TService Activate<TService>(this IServiceProvider serviceProvider, params object[] withParameters)
        => (TService)Activate(serviceProvider, typeof(TService), withParameters);

    public static object Activate(this IServiceProvider serviceProvider, Type serviceType, params object[] withParameters)
    {
        EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));
        Ensure.That(withParameters).DoesNotContainNull();

        var eligibleConstructors = new List<(ConstructorInfo Constructor, object[] Parameters)>();

        var publicParameterlessConstructor = serviceType.GetConstructor(Type.EmptyTypes);

        if (publicParameterlessConstructor != null)
            return publicParameterlessConstructor.Invoke(Array.Empty<object>());

        foreach (var ctor in serviceType.GetConstructors())
        {
            var ctorParamInfos = ctor.GetParameters();

            var ctorParams = new object[ctorParamInfos.Length];

            for (var i = 0; i < ctorParamInfos.Length; i++)
            {
                var paramType = ctorParamInfos[i].ParameterType;

                ctorParams[i] = withParameters
                    .FirstOrDefault(x => paramType.IsAssignableFrom(x.GetType()))
                    ?? serviceProvider.GetService(paramType);

                if (ctorParams[i] == null) break;
            }

            if (ctorParams.Last() == null) continue;

            eligibleConstructors.Add((ctor, ctorParams));
        }

        if (eligibleConstructors.Count == 0)
            throw new InvalidOperationException($"No public constructor found for type {serviceType} with parameters that can be fulfilled by supplied parameters or the service provider.");

        if (eligibleConstructors.Count > 1)
            throw new InvalidOperationException($"Multiple constructors found for type {serviceType} with parameters that can be fulfilled by supplied parameters or the service provider.");

        var (constructor, parameters) = eligibleConstructors.Single();

        return constructor.Invoke(parameters);
    }
}
