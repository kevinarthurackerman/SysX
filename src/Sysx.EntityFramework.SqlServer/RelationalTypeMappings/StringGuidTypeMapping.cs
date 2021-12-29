namespace Sysx.EntityFramework.SqlServer.RelationalTypeMappings;

public class StringGuidTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<StringGuid, string> converter
       = new(x => x.ToString("D"), x => new StringGuid(Guid.ParseExact(x, "D")));

    public override Type ClrType => typeof(StringGuid);
    public override ValueConverter Converter => converter;
    public override string StoreType => "char(36)";
    public override DbType? DbType => System.Data.DbType.AnsiStringFixedLength;
    public override int? Size => 36;
    public override bool IsFixedLength => true;
    public override bool IsUnicode => false;

    public StringGuidTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new StringGuidTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return $"'{value}'";
    }
}