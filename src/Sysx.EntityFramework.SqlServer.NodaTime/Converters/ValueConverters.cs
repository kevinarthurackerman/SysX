namespace Sysx.EntityFramework.SqlServer.NodaTime.Converters;

internal static class ValueConverters
{
    internal static ValueConverter Duration =
        new ValueConverter<_NodaTime.Duration, TimeSpan>(x => x.ToTimeSpan(), x => _NodaTime.Duration.FromTimeSpan(x));

    internal static ValueConverter Instant =
        new ValueConverter<_NodaTime.Instant, DateTime>(x => x.ToDateTimeUtc(), x => _NodaTime.Instant.FromDateTimeUtc(x));

    internal static ValueConverter LocalDateTime =
        new ValueConverter<_NodaTime.LocalDateTime, DateTime>(x => x.ToDateTimeUnspecified(), x => _NodaTime.LocalDateTime.FromDateTime(x));

    internal static ValueConverter LocalDate =
        new ValueConverter<_NodaTime.LocalDate, DateTime>(x => x.ToDateTimeUnspecified(), x => _NodaTime.LocalDate.FromDateTime(x));

    internal static ValueConverter LocalTime =
        new ValueConverter<_NodaTime.LocalTime, TimeSpan>(x => TimeSpan.FromTicks(x.TickOfDay), x => _NodaTime.LocalTime.FromTicksSinceMidnight(x.Ticks));

    internal static ValueConverter OffsetDateTime =
        new ValueConverter<_NodaTime.OffsetDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => _NodaTime.OffsetDateTime.FromDateTimeOffset(x));

    internal static ValueConverter Offset =
        new ValueConverter<_NodaTime.Offset, int>(x => x.Seconds, x => _NodaTime.Offset.FromSeconds(x));

    internal static ValueConverter ZonedDateTime =
        new ValueConverter<_NodaTime.ZonedDateTime, DateTimeOffset>(x => x.ToDateTimeOffset(), x => _NodaTime.ZonedDateTime.FromDateTimeOffset(x));
}
