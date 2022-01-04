namespace Sysx.EntityFramework.Plugins.SequentialGuids;

/// <summary>
/// ContainerTypesDbContextOptionsExtension that adds handling of sequential GUID types to EntityFramework
/// </summary>
public class SequentialGuidDbContextOptionsExtension : BaseContainerTypesDbContextOptionsExtension
{
    public SequentialGuidDbContextOptionsExtension() : base("SequentialGuid") { }

    public override void ApplyServices(IServiceCollection services)
    {
        EnsureArg.IsNotNull(services, nameof(services));

        base.ApplyServices(services);

        services.AddScoped<RelationalTypeMapping>(services =>
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

            return new LazyInitializedRelationalTypeMapping<BinaryGuid>(initializeRelationalTypeMapper);
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

            return new LazyInitializedRelationalTypeMapping<StringGuid>(initializeRelationalTypeMapper);
        });

        services.AddScoped<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(Guid));

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(SqlServerGuid), size: 36, fixedLength: true))
                    .Clone(new ValueConverter<SqlServerGuid, Guid>(x => (Guid)x, x => (SqlServerGuid)x));
            };

            return new LazyInitializedRelationalTypeMapping<SqlServerGuid>(initializeRelationalTypeMapper);
        });
    }
}