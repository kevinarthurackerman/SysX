namespace Sysx.EntityFramework.SqlServer.RelationalTypeMappings;

public class LocalDateTimeTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<LocalDateTime, DateTime> converter =
        new(x => x.ToDateTimeUnspecified(), x => LocalDateTime.FromDateTime(x));

    public override Type ClrType => typeof(LocalDateTime);
    public override ValueConverter Converter => converter;
    public override string StoreType => "datetime2";
    public override DbType? DbType => System.Data.DbType.DateTime2;
    public override int? Size => null;
    public override bool IsFixedLength => false;

    public LocalDateTimeTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new LocalDateTimeTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}