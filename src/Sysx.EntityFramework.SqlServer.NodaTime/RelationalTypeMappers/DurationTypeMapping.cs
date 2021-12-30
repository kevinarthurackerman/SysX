namespace Sysx.EntityFramework.SqlServer.RelationalTypeMappings;

/// <summary>
/// Type mapping for Durations
/// </summary>
public class DurationTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<Duration, long> converter =
        new(x => x.BclCompatibleTicks, x => Duration.FromTicks(x));

    public override Type ClrType => typeof(Duration);
    public override ValueConverter Converter => converter;
    public override string StoreType => "bigint";
    public override DbType? DbType => System.Data.DbType.Int64;
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