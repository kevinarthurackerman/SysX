namespace Sysx.EntityFramework.Plugins.Identifiers;

/// <summary>
/// ContainerTypesDbContextOptionsExtension that adds handling of identifier types to EntityFramework
/// </summary>
public class IdentifiersDbContextOptionsExtension : BaseContainerTypesDbContextOptionsExtension
{
    public IdentifiersDbContextOptionsExtension() : base("Identifiers") { }

    public override void ApplyServices(IServiceCollection services)
    {
        EnsureArg.IsNotNull(services, nameof(services));

        base.ApplyServices(services);

        services.AddSingleton<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(byte[]));

                var storeType = providerTypeMapping.StoreType.Equals("varbinary(max)", StringComparison.OrdinalIgnoreCase)
                    ? "binary(16)"
                    : providerTypeMapping.StoreType;

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(BinaryGuid), storeTypeName: storeType))
                    .Clone(new ValueConverter<BinaryGuid, byte[]>(x => x.ToByteArray(), x => new BinaryGuid(new Guid(x))));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(BinaryGuid), initializeRelationalTypeMapper);
        });
        
        services.AddScoped<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(string));

                var storeType = providerTypeMapping.StoreType.Equals("nvarchar(max)", StringComparison.OrdinalIgnoreCase)
                    ? "char(36)"
                    : providerTypeMapping.StoreType;

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(StringGuid), storeTypeName: storeType))
                    .Clone(new ValueConverter<StringGuid, string>(x => x.ToString("D"), x => new StringGuid(Guid.ParseExact(x, "D"))));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(StringGuid), initializeRelationalTypeMapper);
        });

        services.AddScoped<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(Guid));

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(SqlServerGuid)))
                    .Clone(new ValueConverter<SqlServerGuid, Guid>(x => (Guid)x, x => (SqlServerGuid)x));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(SqlServerGuid), initializeRelationalTypeMapper);
        });
    }
}