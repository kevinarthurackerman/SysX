namespace Sysx.EntityFramework.Sqlite.RelationalTypeMappings;

/// <summary>
/// Type mapping for LocalDates
/// </summary>
public class LocalDateTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<LocalDate, string> converter = new(
            x => x.ToDateTimeUnspecified().ToString(@"yyyy\-MM\-dd"),
            x => LocalDate.FromDateTime(DateTime.ParseExact(x, @"yyyy\-MM\-dd", CultureInfo.InvariantCulture))
        );

    public override Type ClrType => typeof(LocalDate);
    public override ValueConverter Converter => converter;
    public override string StoreType => "TEXT";
    public override DbType? DbType => System.Data.DbType.Date;
    public override int? Size => 10;
    public override bool IsFixedLength => true;

    public LocalDateTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new LocalDateTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}