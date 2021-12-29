namespace Sysx.EntityFramework.Sqlite.RelationalTypeMappings;

public class OffsetDateTimeTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<OffsetDateTime, string> converter = new(
            x => x.ToDateTimeOffset().ToString(@"yyyy\-MM\-dd\ HH\:mm\:ss\.FFFFFFFzzz"),
            x => OffsetDateTime.FromDateTimeOffset(DateTimeOffset.ParseExact(x, @"yyyy\-MM\-dd\ HH\:mm\:ss\.FFFFFFFzzz", CultureInfo.InvariantCulture))
        );

    public override Type ClrType => typeof(OffsetDateTime);
    public override ValueConverter Converter => converter;
    public override string StoreType => "TEXT";
    public override DbType? DbType => System.Data.DbType.DateTimeOffset;
    public override int? Size => 33;
    public override bool IsFixedLength => true;

    public OffsetDateTimeTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new OffsetDateTimeTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}