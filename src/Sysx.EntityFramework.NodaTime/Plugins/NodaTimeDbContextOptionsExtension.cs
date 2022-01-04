namespace Sysx.EntityFramework.NodaTime.Plugins;

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

        services.AddSingleton<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(long));

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(Duration)))
                    .Clone(new ValueConverter<Duration, long>(x => x.BclCompatibleTicks, x => Duration.FromTicks(x)));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(Duration), initializeRelationalTypeMapper);
        });

        services.AddSingleton<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(DateTime));

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(Instant)))
                    .Clone(new ValueConverter<Instant, DateTime>(x => x.ToDateTimeUtc(), x => Instant.FromDateTimeUtc(x)));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(Instant), initializeRelationalTypeMapper);
        });

        services.AddSingleton<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(DateTime));

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(LocalDateTime)))
                    .Clone(new ValueConverter<LocalDateTime, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDateTime.FromDateTime(x)));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(LocalDateTime), initializeRelationalTypeMapper);
        });

        services.AddSingleton<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(DateTime));

                var storeType = providerTypeMapping.StoreType.Equals("datetime2", StringComparison.OrdinalIgnoreCase)
                    ? "date"
                    : providerTypeMapping.StoreType;

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(LocalDate), storeTypeName: storeType))
                    .Clone(new ValueConverter<LocalDate, DateTime>(x => x.ToDateTimeUnspecified(), x => LocalDate.FromDateTime(x)));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(LocalDate), initializeRelationalTypeMapper);
        });

        services.AddSingleton<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(TimeSpan));

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(LocalTime)))
                    .Clone(new ValueConverter<LocalTime, TimeSpan>(x => TimeSpan.FromTicks(x.TickOfDay), x => LocalTime.FromTicksSinceMidnight(x.Ticks)));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(LocalTime), initializeRelationalTypeMapper);
        });

        services.AddSingleton<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(DateTimeOffset));

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(OffsetDateTime)))
                    .Clone(new ValueConverter<OffsetDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => OffsetDateTime.FromDateTimeOffset(x)));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(OffsetDateTime), initializeRelationalTypeMapper);
        });

        services.AddSingleton<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(int));

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(Offset)))
                    .Clone(new ValueConverter<Offset, int>(x => x.Seconds, x => Offset.FromSeconds(x)));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(Offset), initializeRelationalTypeMapper);
        });

        services.AddSingleton<RelationalTypeMapping>(services =>
        {
            var initializeRelationalTypeMapper = () =>
            {
                var providerTypeMapping = services
                    .GetRequiredService<IRelationalTypeMappingSource>()
                    .FindMapping(typeof(DateTimeOffset));

                return (RelationalTypeMapping)providerTypeMapping
                    .Clone(new RelationalTypeMappingInfo(typeof(ZonedDateTime)))
                    .Clone(new ValueConverter<ZonedDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => ZonedDateTime.FromDateTimeOffset(x)));
            };

            return new LazyInitializedRelationalTypeMapping(typeof(ZonedDateTime), initializeRelationalTypeMapper);
        });
    }
}