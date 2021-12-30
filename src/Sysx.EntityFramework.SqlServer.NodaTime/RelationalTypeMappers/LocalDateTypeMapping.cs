namespace Sysx.EntityFramework.SqlServer.RelationalTypeMappings;

/// <summary>
/// Type mapping for LocalDates
/// </summary>
public class LocalDateTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<LocalDate, DateTime> converter =
        new(x => x.ToDateTimeUnspecified(), x => LocalDate.FromDateTime(x));

    public override Type ClrType => typeof(LocalDate);
    public override ValueConverter Converter => converter;
    public override string StoreType => "date";
    public override DbType? DbType => System.Data.DbType.Date;
    public override int? Size => null;
    public override bool IsFixedLength => false;

    public LocalDateTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new LocalDateTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}