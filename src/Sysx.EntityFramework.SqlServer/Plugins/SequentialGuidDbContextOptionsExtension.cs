namespace Sysx.EntityFramework.SqlServer.Plugins;

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

        services.UpsertScoped<RelationalTypeMapping, BinaryGuidTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, StringGuidTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, SqlServerGuidTypeMapping>();
    }
}