namespace SysX.EntityFramework.Plugins;

public sealed class LazyInitializedRelationalTypeMapping : RelationalTypeMapping
{
	private readonly Type clrType;
	private readonly Lazy<RelationalTypeMapping> innerRelationalTypeMapper;

	public override Type ClrType => clrType;

	public override ValueComparer Comparer => innerRelationalTypeMapper.Value.Comparer;

	public override ValueComparer KeyComparer => innerRelationalTypeMapper.Value.KeyComparer;

	public override ValueConverter Converter => innerRelationalTypeMapper.Value.Converter;

	public override Func<IProperty, IEntityType, ValueGenerator> ValueGeneratorFactory =>
		innerRelationalTypeMapper.Value.ValueGeneratorFactory;

	public override StoreTypePostfix StoreTypePostfix =>
		innerRelationalTypeMapper.Value.StoreTypePostfix;

	public override string StoreType => innerRelationalTypeMapper.Value.StoreType;

	public override string StoreTypeNameBase => innerRelationalTypeMapper.Value.StoreTypeNameBase;

	public override DbType? DbType => innerRelationalTypeMapper.Value.DbType;

	public override bool IsUnicode => innerRelationalTypeMapper.Value.IsUnicode;

	public override int? Size => innerRelationalTypeMapper.Value.Size;

	public override int? Precision => innerRelationalTypeMapper.Value.Precision;

	public override int? Scale => innerRelationalTypeMapper.Value.Scale;

	public override bool IsFixedLength => innerRelationalTypeMapper.Value.IsFixedLength;

	[Obsolete("Use KeyComparer. Key comparers must implement structural comparisons and deep copies.")]
	public override ValueComparer StructuralComparer => innerRelationalTypeMapper.Value.StructuralComparer;

	public LazyInitializedRelationalTypeMapping(Type clrType, Func<RelationalTypeMapping> initializeRelationalTypeMapping)
		: base(string.Empty, typeof(object))
	{
		EnsureArg.IsNotNull(clrType, nameof(clrType));
		EnsureArg.IsNotNull(initializeRelationalTypeMapping, nameof(initializeRelationalTypeMapping));

		this.clrType = clrType;
		innerRelationalTypeMapper = new(initializeRelationalTypeMapping);
	}

	public override DbParameter CreateParameter(DbCommand command, string name, object? value, bool? nullable = null) =>
		innerRelationalTypeMapper.Value.CreateParameter(command, name, value, nullable);

	public override string GenerateSqlLiteral(object? value) =>
		innerRelationalTypeMapper.Value.GenerateSqlLiteral(value);

	public override string GenerateProviderValueSqlLiteral(object? value) =>
		innerRelationalTypeMapper.Value.GenerateProviderValueSqlLiteral(value);

	public override MethodInfo GetDataReaderMethod() =>
		innerRelationalTypeMapper.Value.GetDataReaderMethod();

	public override Expression GenerateCodeLiteral(object value) =>
		innerRelationalTypeMapper.Value.GenerateCodeLiteral(value);

	public override RelationalTypeMapping Clone(string storeType, int? size) =>
		innerRelationalTypeMapper.Value.Clone(storeType, size);

	public override RelationalTypeMapping Clone(int? precision, int? scale) =>
		innerRelationalTypeMapper.Value.Clone(precision, scale);

	public override RelationalTypeMapping Clone(in RelationalTypeMappingInfo mappingInfo) =>
		innerRelationalTypeMapper.Value.Clone(in mappingInfo);

	public override CoreTypeMapping Clone(ValueConverter? converter) =>
		innerRelationalTypeMapper.Value.Clone(converter);

	public override Expression CustomizeDataReaderExpression(Expression expression) =>
		innerRelationalTypeMapper.Value.CustomizeDataReaderExpression(expression);

	public override bool Equals(object? obj) => innerRelationalTypeMapper.Value.Equals(obj);

	public override int GetHashCode() => innerRelationalTypeMapper.Value.GetHashCode();

	public override string? ToString() => innerRelationalTypeMapper.Value.ToString();

	protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) =>
		throw new NotImplementedException();
}