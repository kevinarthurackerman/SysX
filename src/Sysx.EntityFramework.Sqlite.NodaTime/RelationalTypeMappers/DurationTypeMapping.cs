namespace Sysx.EntityFramework.Sqlite.RelationalTypeMappings;

/// <summary>
/// Type mapping for Durations
/// </summary>
public class DurationTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<Duration, string> converter = new (
            x => x.ToTimeSpan().ToString(@"d\.hh\:mm\:ss\.fffffff"),
            x => Duration.FromTimeSpan(TimeSpan.ParseExact(x, @"d\.hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture))
        );

    public override Type ClrType => typeof(Duration);
    public override ValueConverter Converter => converter;
    public override string StoreType => "TEXT";
    public override DbType? DbType => System.Data.DbType.Time;
    public override int? Size => null;
    public override bool IsFixedLength => false;

    public DurationTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new DurationTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}