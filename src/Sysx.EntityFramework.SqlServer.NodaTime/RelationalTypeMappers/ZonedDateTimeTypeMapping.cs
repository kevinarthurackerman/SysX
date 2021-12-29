namespace Sysx.EntityFramework.SqlServer.RelationalTypeMappings;

public class ZonedDateTimeTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<ZonedDateTime, DateTimeOffset> converter =
        new(x => x.ToDateTimeOffset(), x => ZonedDateTime.FromDateTimeOffset(x));

    public override Type ClrType => typeof(ZonedDateTime);
    public override ValueConverter Converter => converter;
    public override string StoreType => "datetimeoffset";
    public override DbType? DbType => System.Data.DbType.DateTimeOffset;
    public override int? Size => null;
    public override bool IsFixedLength => false;

    public ZonedDateTimeTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new ZonedDateTimeTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}