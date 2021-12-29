namespace Sysx.EntityFramework.SqlServer.RelationalTypeMappings;

public class LocalTimeTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<LocalTime, TimeSpan> converter = 
        new(x => TimeSpan.FromTicks(x.TickOfDay), x => LocalTime.FromTicksSinceMidnight(x.Ticks));

    public override Type ClrType => typeof(LocalTime);
    public override ValueConverter Converter => converter;
    public override string StoreType => "time";
    public override DbType? DbType => System.Data.DbType.Time;
    public override int? Size => null;
    public override bool IsFixedLength => false;

    public LocalTimeTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new LocalTimeTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}