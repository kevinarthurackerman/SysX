namespace Sysx.EntityFramework.Sqlite.RelationalTypeMappings;

/// <summary>
/// Type mapping for SqlServerGuids
/// </summary>
public class SqlServerGuidTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<SqlServerGuid, Guid> converter
       = new(x => (Guid)x, x => (SqlServerGuid)x);

    public override Type ClrType => typeof(SqlServerGuid);
    public override ValueConverter Converter => converter;
    public override string StoreType => "BLOB";
    public override DbType? DbType => System.Data.DbType.Guid;

    public SqlServerGuidTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new SqlServerGuidTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        return $"X'{value}'";
    }
}