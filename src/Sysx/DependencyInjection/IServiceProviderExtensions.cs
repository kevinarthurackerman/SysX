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
            throw new InvalidOperationException($"A suitable constructor for type '{serviceType}' could not be located. Ensure the type is concrete and services are registered or provided via {nameof(withParameters)} for all parameters of a public constructor.");

        if (eligibleConstructors.Count > 1)
            throw new InvalidOperationException($"Multiple constructors accepting all given argument types have been found in type '{serviceType}'. There should only be one applicable constructor.");

        var (constructor, parameters) = eligibleConstructors.Single();

        return constructor.Invoke(parameters);
    }
}
