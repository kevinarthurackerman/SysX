namespace Sysx.EntityFramework.SqlServer.NodaTime;

public static class ModelBuilderExtensions
{
    public static ModelBuilder RegisterNodaTimeConversions(this ModelBuilder modelBuilder) =>
        modelBuilder.RegisterDurations()
            .RegisterInstants()
            .RegisterLocalDateTimes()
            .RegisterLocalDates()
            .RegisterLocalTimes()
            .RegisterOffsetDateTimes()
            .RegisterOffsets()
            .RegisterZonedDateTimes();

    public static ModelBuilder RegisterDurations(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(TimeSpan));
            entityProp.SetValueConverter(ValueConverters.Duration);
        };

        modelBuilder.RegisterPropertiesOfType<_NodaTime.Duration>(registerType);
        modelBuilder.RegisterPropertiesOfType<_NodaTime.Duration?>(registerType);

        return modelBuilder;
    }

    public static ModelBuilder RegisterInstants(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(DateTime));
            entityProp.SetValueConverter(ValueConverters.Instant);
        };

        modelBuilder.RegisterPropertiesOfType<_NodaTime.Instant>(registerType);
        modelBuilder.RegisterPropertiesOfType<_NodaTime.Instant?>(registerType);

        return modelBuilder;
    }

    public static ModelBuilder RegisterLocalDateTimes(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(DateTime));
            entityProp.SetValueConverter(ValueConverters.LocalDateTime);
        };
        
        modelBuilder.RegisterPropertiesOfType<_NodaTime.LocalDateTime>(registerType);
        modelBuilder.RegisterPropertiesOfType<_NodaTime.LocalDateTime?>(registerType);

        return modelBuilder;
    }

    public static ModelBuilder RegisterLocalDates(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(DateTime));
            entityProp.SetColumnType("date");
            entityProp.SetValueConverter(ValueConverters.LocalDate);
        };

        modelBuilder.RegisterPropertiesOfType<_NodaTime.LocalDate>(registerType);
        modelBuilder.RegisterPropertiesOfType<_NodaTime.LocalDate?>(registerType);

        return modelBuilder;
    }

    public static ModelBuilder RegisterLocalTimes(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(TimeSpan));
            entityProp.SetValueConverter(ValueConverters.LocalTime);
        };

        modelBuilder.RegisterPropertiesOfType<_NodaTime.LocalTime>(registerType);
        modelBuilder.RegisterPropertiesOfType<_NodaTime.LocalTime?>(registerType);

        return modelBuilder;
    }

    public static ModelBuilder RegisterOffsetDateTimes(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(DateTimeOffset));
            entityProp.SetValueConverter(ValueConverters.OffsetDateTime);
        };

        modelBuilder.RegisterPropertiesOfType<_NodaTime.OffsetDateTime>(registerType);
        modelBuilder.RegisterPropertiesOfType<_NodaTime.OffsetDateTime?>(registerType);

        return modelBuilder;
    }

    public static ModelBuilder RegisterOffsets(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(int));
            entityProp.SetValueConverter(ValueConverters.Offset);
        };

        modelBuilder.RegisterPropertiesOfType<_NodaTime.Offset>(registerType);
        modelBuilder.RegisterPropertiesOfType<_NodaTime.Offset?>(registerType);

        return modelBuilder;
    }

    public static ModelBuilder RegisterZonedDateTimes(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(DateTimeOffset));
            entityProp.SetValueConverter(ValueConverters.ZonedDateTime);
        };

        modelBuilder.RegisterPropertiesOfType<_NodaTime.ZonedDateTime>(registerType);
        modelBuilder.RegisterPropertiesOfType<_NodaTime.ZonedDateTime?>(registerType);

        return modelBuilder;
    }
}