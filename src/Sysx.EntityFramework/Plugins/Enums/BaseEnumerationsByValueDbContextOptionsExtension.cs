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

        foreach (var type in scanAssembly.GetTypes())
        {
            var baseEnumerationType = GetBaseEnumerationType(type);

            if (baseEnumerationType == null) continue;

            var genericParams = baseEnumerationType.GetGenericArguments();
            var enumType = genericParams[0];
            var valueType = genericParams[1];

            services.AddSingleton<RelationalTypeMapping>(services =>
            {
                var initializeRelationalTypeMapper = () =>
                {
                    var providerTypeMapping = services
                        .GetRequiredService<IRelationalTypeMappingSource>()
                        .FindMapping(valueType);

                    var valueConverter = (ValueConverter)GetType()
                        .GetMethod(nameof(CreateValueConverter), BindingFlags.NonPublic | BindingFlags.Static)!
                        .MakeGenericMethod(enumType, valueType)
                        .Invoke(null, null)!;

                    return (RelationalTypeMapping)providerTypeMapping
                        .Clone(new RelationalTypeMappingInfo(type))
                        .Clone(valueConverter);
                };

                return new LazyInitializedRelationalTypeMapping(type, initializeRelationalTypeMapper);
            });
        }
    }

    private static Type? GetBaseEnumerationType(Type type)
    {
        var baseType = type!;

        while (baseType != null && baseType != baseType.BaseType)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(BaseEnumeration<,>))
            {
                return baseType;
            }

            baseType = baseType.BaseType;
        }

        return null;
    }

    private static ValueConverter CreateValueConverter<TEnum, TValue>()
        where TEnum : BaseEnumeration<TEnum, TValue>
        where TValue : IComparable<TValue>, IEquatable<TValue>, IComparable
    {
        return new ValueConverter<TEnum, TValue>(
            @enum => @enum.Value,
            value => BaseEnumeration<TEnum, TValue>.ParseValue(value)
        );
    }
}