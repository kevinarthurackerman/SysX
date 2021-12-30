namespace Sysx.EntityFramework.SqlServer.RelationalTypeMappings;

/// <summary>
/// Type mapping for OffsetDateTimes
/// </summary>
public class OffsetDateTimeTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<OffsetDateTime, DateTimeOffset> converter =
        new(x => x.ToDateTimeOffset(), x => OffsetDateTime.FromDateTimeOffset(x));

    public override Type ClrType => typeof(OffsetDateTime);
    public override ValueConverter Converter => converter;
    public override string StoreType => "datetimeoffset";
    public override DbType? DbType => System.Data.DbType.DateTimeOffset;
    public override int? Size => null;
    public override bool IsFixedLength => false;

    public OffsetDateTimeTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new OffsetDateTimeTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}