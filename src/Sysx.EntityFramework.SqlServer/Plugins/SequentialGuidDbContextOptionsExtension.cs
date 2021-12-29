namespace Sysx.EntityFramework.SqlServer.Plugins;

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