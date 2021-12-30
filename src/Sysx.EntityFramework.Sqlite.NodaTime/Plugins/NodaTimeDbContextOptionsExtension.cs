namespace Sysx.EntityFramework.Sqlite.NodaTime.Plugins;

/// <summary>
/// ContainerTypesDbContextOptionsExtension that adds handling of NodaTime types to EntityFramework
/// </summary>
public class NodaTimeDbContextOptionsExtension : BaseContainerTypesDbContextOptionsExtension
{
    public NodaTimeDbContextOptionsExtension() : base("NodaTime") { }

    public override void ApplyServices(IServiceCollection services)
    {
        EnsureArg.IsNotNull(services, nameof(services));

        base.ApplyServices(services);

        services.UpsertScoped<RelationalTypeMapping, DurationTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, InstantTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, LocalDateTimeTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, LocalDateTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, LocalTimeTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, OffsetDateTimeTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, OffsetTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, ZonedDateTimeTypeMapping>();
    }
}