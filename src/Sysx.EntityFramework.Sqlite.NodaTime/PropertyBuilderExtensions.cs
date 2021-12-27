namespace Sysx.EntityFramework.Sqlite.NodaTime;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder IsDuration(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(string))
            .HasConversion(ValueConverters.Duration);

    public static PropertyBuilder IsInstant(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(string))
            .HasConversion(ValueConverters.Instant);

    public static PropertyBuilder IsLocalDateTime(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(string))
            .HasConversion(ValueConverters.LocalDateTime);

    public static PropertyBuilder IsLocalDate(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(string))
            .HasConversion(ValueConverters.LocalDate);

    public static PropertyBuilder IsLocalTime(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(string))
            .HasConversion(ValueConverters.LocalTime);

    public static PropertyBuilder IsOffsetDateTime(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(string))
            .HasConversion(ValueConverters.OffsetDateTime);

    public static PropertyBuilder IsOffset(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(string))
            .HasConversion(ValueConverters.Offset);

    public static PropertyBuilder IsZonedDateTime(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(string))
            .HasConversion(ValueConverters.ZonedDateTime);
}