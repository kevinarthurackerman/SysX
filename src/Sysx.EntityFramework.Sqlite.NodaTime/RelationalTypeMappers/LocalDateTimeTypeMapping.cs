namespace Sysx.EntityFramework.Sqlite.RelationalTypeMappings;

public class LocalDateTimeTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<LocalDateTime, string> converter = new(
            x => x.ToDateTimeUnspecified().ToString(@"yyyy\-MM\-dd\ HH\:mm\:ss\.FFFFFFF"),
            x => LocalDateTime.FromDateTime(DateTime.ParseExact(x, @"yyyy\-MM\-dd\ HH\:mm\:ss\.FFFFFFF", CultureInfo.InvariantCulture))
        );

    public override Type ClrType => typeof(LocalDateTime);
    public override ValueConverter Converter => converter;
    public override string StoreType => "TEXT";
    public override DbType? DbType => System.Data.DbType.DateTime2;
    public override int? Size => 27;
    public override bool IsFixedLength => true;

    public LocalDateTimeTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new LocalDateTimeTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value) 
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}