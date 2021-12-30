namespace Sysx.EntityFramework.SqlServer.RelationalTypeMappings;

/// <summary>
/// Type mapping for Instants
/// </summary>
public class InstantTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<Instant, DateTime> converter = 
        new(x => x.ToDateTimeUtc(), x => Instant.FromDateTimeUtc(x));

    public override Type ClrType => typeof(Instant);
    public override ValueConverter Converter => converter;
    public override string StoreType => "datetime2";
    public override DbType? DbType => System.Data.DbType.DateTime2;
    public override int? Size => null;
    public override bool IsFixedLength => false;

    public InstantTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new InstantTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}