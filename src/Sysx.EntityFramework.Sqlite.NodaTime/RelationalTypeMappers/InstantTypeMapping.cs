namespace Sysx.EntityFramework.Sqlite.RelationalTypeMappings;

/// <summary>
/// Type mapping for Instants
/// </summary>
public class InstantTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<Instant, string> converter = new (
            x => x.ToDateTimeUtc().ToString(@"yyyy\-MM\-dd\ HH\:mm\:ss\.FFFFFFF"),
            x => Instant.FromDateTimeUtc(DateTime.ParseExact(x, @"yyyy\-MM\-dd\ HH\:mm\:ss\.FFFFFFF", CultureInfo.InvariantCulture))
        );

    public override Type ClrType => typeof(Instant);
    public override ValueConverter Converter => converter;
    public override string StoreType => "TEXT";
    public override DbType? DbType => System.Data.DbType.Time;
    public override int? Size => 27;
    public override bool IsFixedLength => true;

    public InstantTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new InstantTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}