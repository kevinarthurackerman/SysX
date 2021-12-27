namespace Sysx.EntityFramework.SqlServer.NodaTime;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder IsDuration(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(TimeSpan))
            .HasConversion(ValueConverters.Duration);

    public static PropertyBuilder IsInstant(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(DateTime))
            .HasConversion(ValueConverters.Instant);

    public static PropertyBuilder IsLocalDateTime(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(DateTime))
            .HasConversion(ValueConverters.LocalDateTime);

    public static PropertyBuilder IsLocalDate(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(DateTime))
            .HasColumnType("date")
            .HasConversion(ValueConverters.LocalDate);

    public static PropertyBuilder IsLocalTime(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(TimeSpan))
            .HasConversion(ValueConverters.LocalTime);

    public static PropertyBuilder IsOffsetDateTime(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(DateTimeOffset))
            .HasConversion(ValueConverters.OffsetDateTime);

    public static PropertyBuilder IsOffset(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(int))
            .HasConversion(ValueConverters.Offset);

    public static PropertyBuilder IsZonedDateTime(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(DateTimeOffset))
            .HasConversion(ValueConverters.ZonedDateTime);
}