namespace Sysx.EntityFramework.Sqlite.NodaTime.Converters;

internal static class ValueConverters
{
    private const string timeSpanFormat = @"d\.hh\:mm\:ss\.fffffff";
    private const string datetimeFormat = @"yyyy\-MM\-dd\ HH\:mm\:ss\.FFFFFFF";
    private const string dateFormat = @"yyyy\-MM\-dd";
    private const string timeFormat = @"hh\:mm\:ss\.fffffff";
    private const string datetimeOffsetFormat = @"yyyy\-MM\-dd\ HH\:mm\:ss\.FFFFFFFzzz";

    internal static ValueConverter Duration =
        new ValueConverter<_NodaTime.Duration, string>(
            x => x.ToTimeSpan().ToString(timeSpanFormat),
            x => _NodaTime.Duration.FromTimeSpan(TimeSpan.ParseExact(x, timeSpanFormat, CultureInfo.InvariantCulture))
        );

    internal static ValueConverter Instant =
        new ValueConverter<_NodaTime.Instant, string>(
            x => x.ToDateTimeUtc().ToString(datetimeFormat),
            x => _NodaTime.Instant.FromDateTimeUtc(DateTime.ParseExact(x, datetimeFormat, CultureInfo.InvariantCulture))
        );

    internal static ValueConverter LocalDateTime =
        new ValueConverter<_NodaTime.LocalDateTime, string>(
            x => x.ToDateTimeUnspecified().ToString(datetimeFormat),
            x => _NodaTime.LocalDateTime.FromDateTime(DateTime.ParseExact(x, datetimeFormat, CultureInfo.InvariantCulture))
        );

    internal static ValueConverter LocalDate =
        new ValueConverter<_NodaTime.LocalDate, string>(
            x => x.ToDateTimeUnspecified().ToString(dateFormat),
            x => _NodaTime.LocalDate.FromDateTime(DateTime.ParseExact(x, dateFormat, CultureInfo.InvariantCulture))
        );

    internal static ValueConverter LocalTime =
        new ValueConverter<_NodaTime.LocalTime, string>(
            x => TimeSpan.FromTicks(x.TickOfDay).ToString(timeFormat),
            x => _NodaTime.LocalTime.FromTicksSinceMidnight(TimeSpan.ParseExact(x, timeFormat, CultureInfo.InvariantCulture).Ticks)
        );

    internal static ValueConverter OffsetDateTime =
        new ValueConverter<_NodaTime.OffsetDateTime, string>(
            x => x.ToDateTimeOffset().ToString(datetimeOffsetFormat),
            x => _NodaTime.OffsetDateTime.FromDateTimeOffset(DateTimeOffset.ParseExact(x, datetimeOffsetFormat, CultureInfo.InvariantCulture))
        );

    internal static ValueConverter Offset =
        new ValueConverter<_NodaTime.Offset, string>(
            x => TimeSpan.FromTicks(x.Ticks).ToString(timeFormat),
            x => _NodaTime.Offset.FromTicks(TimeSpan.ParseExact(x, timeFormat, CultureInfo.InvariantCulture).Ticks)
        );

    internal static ValueConverter ZonedDateTime =
        new ValueConverter<_NodaTime.ZonedDateTime, string>(
            x => x.ToDateTimeOffset().ToString(datetimeOffsetFormat),
            x => _NodaTime.ZonedDateTime.FromDateTimeOffset(DateTimeOffset.ParseExact(x, datetimeOffsetFormat, CultureInfo.InvariantCulture))
        );
}
