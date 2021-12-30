namespace Sysx.EntityFramework.Sqlite.RelationalTypeMappings;

/// <summary>
/// Type mapping for LocalTimes
/// </summary>
public class LocalTimeTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<LocalTime, string> converter = new(
            x => TimeSpan.FromTicks(x.TickOfDay).ToString(@"hh\:mm\:ss\.fffffff"),
            x => LocalTime.FromTicksSinceMidnight(TimeSpan.ParseExact(x, @"hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture).Ticks)
        );

    public override Type ClrType => typeof(LocalTime);
    public override ValueConverter Converter => converter;
    public override string StoreType => "TEXT";
    public override DbType? DbType => System.Data.DbType.Time;
    public override int? Size => 16;
    public override bool IsFixedLength => true;

    public LocalTimeTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new LocalTimeTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}