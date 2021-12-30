namespace Sysx.EntityFramework.SqlServer.RelationalTypeMappings;

/// <summary>
/// Type mapping for Offsets
/// </summary>
public class OffsetTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<Offset, int> converter =
        new(x => x.Seconds, x => Offset.FromSeconds(x));

    public override Type ClrType => typeof(Offset);
    public override ValueConverter Converter => converter;
    public override string StoreType => "int";
    public override DbType? DbType => System.Data.DbType.Int32;
    public override int? Size => null;
    public override bool IsFixedLength => false;

    public OffsetTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new OffsetTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return (string)value;
    }
}