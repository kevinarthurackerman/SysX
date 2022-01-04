namespace Sysx.EntityFramework.Plugins.Enums;

/// <summary>
/// ContainerTypesDbContextOptionsExtension that adds handling of enumeration types to EntityFramework
/// </summary>
public class BaseEnumerationsByValueDbContextOptionsExtension : BaseContainerTypesDbContextOptionsExtension
{
    private readonly Assembly scanAssembly;

    public BaseEnumerationsByValueDbContextOptionsExtension(Assembly scanAssembly) : base("Enumerations")
    {
        this.scanAssembly = scanAssembly;
    }

    public override void ApplyServices(IServiceCollection services)
    {
        EnsureArg.IsNotNull(services, nameof(services));

        base.ApplyServices(services);

        var enumTypes = scanAssembly.GetTypes()
            .Where(x => IsEnumerationType(x))
            .ToArray();

        foreach (var enumType in enumTypes)
        {
            services.AddSingleton(typeof(RelationalTypeMapping), services =>
                new BaseEnumerationsByValueTypeMappingFactory(services)
                    .CreateRelationalTypeMapping(enumType));
        }
    }

    private static bool IsEnumerationType(Type type)
    {
        if (type == null) return false;
        if (!type.IsClass) return false;
        if (type.IsAbstract) return false;

        var baseType = type;
        while (baseType != null && baseType != baseType.BaseType)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(BaseEnumeration<,>))
                    return true;

            baseType = baseType.BaseType;
        }

        return false;
    }
}