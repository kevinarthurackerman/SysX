namespace Sysx.EntityFramework.Enums.RelationalTypeMappings;

/// <summary>
/// Factory for producing RelationalTypeMappings for types extending BaseEnumeration.
/// </summary>
public class BaseEnumerationsByValueTypeMappingFactory
{
    private readonly IServiceProvider serviceProvider;

    public BaseEnumerationsByValueTypeMappingFactory(IServiceProvider serviceProvider)
    {
        EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));

        this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates a RelationalTypeMappings for the supplied type. The supplied type must extend BaseEnumeration. 
    /// </summary>
    public RelationalTypeMapping CreateRelationalTypeMapping(Type enumerationType)
    {
        EnsureArg.IsNotNull(enumerationType, nameof(enumerationType));

        var baseType = enumerationType!;

        while (baseType != null && baseType != baseType.BaseType)
        {
            if (!baseType.IsGenericType || baseType.GetGenericTypeDefinition() != typeof(BaseEnumeration<,>))
            {
                baseType = baseType.BaseType;
                continue;
            }

            var enumType = baseType.GenericTypeArguments[0];
            var valueType = baseType.GenericTypeArguments[1];

            return (RelationalTypeMapping)typeof(EnumerationByValueRelationalTypeMapping<,,>)
                .MakeGenericType(enumerationType, enumType, valueType)
                .GetConstructor(new[] { typeof(IServiceProvider) })!
                .Invoke(new object[] { serviceProvider });
        }

        throw new InvalidOperationException($"Could not create {typeof(RelationalTypeMapping)} for type {enumerationType} because it does not extend type {typeof(BaseEnumeration<,>)}.");
    }

    /// <summary>
    /// Wraps the RelationalTypeMapping for the TEnum's underlying TValue type.
    /// Uses a custom ValueConverter to map directly to the TValue type if it has no ValueConverter, or to convert to it to
    /// the TValue type and then use the TValue's ValueConverter to convert to the final type.
    /// Explicitly does not override the following properties as they concern the in-memory type representation:
    /// public ValueComparer Comparer
    /// public ValueComparer KeyComparer
    /// Explicitly does not override the following method as no customization is needed:
    /// public Expression CustomizeDataReaderExpression(Expression expression)
    /// </summary>
    public sealed class EnumerationByValueRelationalTypeMapping<TEnumeration, TEnum, TValue> : BaseRelationalMappingForType
        where TEnumeration : BaseEnumeration<TEnum, TValue>
        where TEnum : BaseEnumeration<TEnum, TValue>
        where TValue : IComparable<TValue>, IEquatable<TValue>, IComparable
    {
        private readonly IServiceProvider serviceProvider;

        private RelationalTypeMapping? innerRelationalTypeMapper;
        private ValueConverter? clrConverter;
        private readonly ValueConverter enumConverter =
            new ValueConverter<TEnum, TValue>(
                (TEnum enumeration) => enumeration.Value,
                (TValue clrValue) => BaseEnumeration<TEnum, TValue>.ParseValue(clrValue)
            );

        private bool isInitialized = false;
        private readonly object @lock = new { };

        public override Type ForType => typeof(TEnumeration);

        public override Type ClrType => typeof(TEnumeration);

        public override ValueConverter Converter
        {
            get
            {
                EnsureInitialized();
                return clrConverter!;
            }
        }

        public override Func<IProperty, IEntityType, ValueGenerator> ValueGeneratorFactory
        {
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.ValueGeneratorFactory;
            }
        }

        public override StoreTypePostfix StoreTypePostfix
        {
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.StoreTypePostfix;
            }
        }

        public override string StoreType
        {
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.StoreType;
            }
        }

        public override string StoreTypeNameBase
        {
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.StoreTypeNameBase;
            }
        }

        public override DbType? DbType
        {
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.DbType;
            }
        }

        public override bool IsUnicode
        {
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.IsUnicode;
            }
        }

        public override int? Size
        {
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.Size;
            }
        }

        public override int? Precision
        {
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.Precision;
            }
        }

        public override int? Scale
        {
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.Scale;
            }
        }

        public override bool IsFixedLength
        {
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.IsFixedLength;
            }
        }

        [Obsolete("Use KeyComparer. Key comparers must implement structural comparisons and deep copies.")]
        public override ValueComparer StructuralComparer
        { 
            get
            {
                EnsureInitialized();
                return innerRelationalTypeMapper!.StructuralComparer;
            } 
        }

        public EnumerationByValueRelationalTypeMapping(IServiceProvider serviceProvider) : base(string.Empty, typeof(TEnumeration))
        {
            EnsureArg.IsNotNull(serviceProvider, nameof(serviceProvider));

            this.serviceProvider = serviceProvider;
        }

        public override DbParameter CreateParameter(DbCommand command, string name, object? value, bool? nullable = null)
        {
            EnsureInitialized();

            var convertedValue = enumConverter.ConvertToProvider(value);

            return innerRelationalTypeMapper!.CreateParameter(command, name, convertedValue, nullable);
        }

        public override string GenerateSqlLiteral(object? value)
        {
            EnsureInitialized();

            var convertedValue = enumConverter.ConvertToProvider(value);

            return innerRelationalTypeMapper!.GenerateSqlLiteral(convertedValue);
        }

        public override string GenerateProviderValueSqlLiteral(object? value)
        {
            EnsureInitialized();

            var convertedValue = enumConverter.ConvertToProvider(value);

            return innerRelationalTypeMapper!.GenerateProviderValueSqlLiteral(convertedValue);
        }

        public override MethodInfo GetDataReaderMethod()
        {
            EnsureInitialized();
            return innerRelationalTypeMapper!.GetDataReaderMethod();
        }

        public override Expression GenerateCodeLiteral(object value)
        {
            EnsureInitialized();

            var convertedValue = enumConverter.ConvertToProvider(value);

            return innerRelationalTypeMapper!.GenerateCodeLiteral(convertedValue);
        }

        public override RelationalTypeMapping Clone(string storeType, int? size) =>
            throw new InvalidOperationException("Cloning this mapper is not supported.");

        public override RelationalTypeMapping Clone(int? precision, int? scale) =>
            throw new InvalidOperationException("Cloning this mapper is not supported.");

        public override RelationalTypeMapping Clone(in RelationalTypeMappingInfo mappingInfo) =>
            throw new InvalidOperationException("Cloning this mapper is not supported.");

        public override CoreTypeMapping Clone(ValueConverter? converter) =>
            throw new InvalidOperationException("Cloning this mapper is not supported.");

        protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) =>
            throw new InvalidOperationException("Cloning this mapper is not supported.");

        private void EnsureInitialized()
        {
            if (!isInitialized)
            {
                lock (@lock)
                {
                    if (isInitialized) return;

                    innerRelationalTypeMapper = serviceProvider.GetRequiredService<IRelationalTypeMappingSource>().FindMapping(typeof(TValue));

                    if (innerRelationalTypeMapper.Converter == null)
                    {
                        clrConverter = enumConverter;
                    }
                    else
                    {
                        var createConverterMethod = GetType()
                            .GetMethod(nameof(CreateValueConverter), BindingFlags.NonPublic | BindingFlags.Static)!
                            .MakeGenericMethod(innerRelationalTypeMapper.Converter.ProviderClrType ?? typeof(TValue));

                        clrConverter = (ValueConverter)createConverterMethod
                            .Invoke(null, new[] { innerRelationalTypeMapper })!;
                    }

                    isInitialized = true;
                }
            }
        }

        private static ValueConverter CreateValueConverter<TProviderType>(RelationalTypeMapping relationalTypeMapping)
        {
            return new ValueConverter<TEnum, TProviderType>(
                (TEnum enumeration) => (TProviderType)relationalTypeMapping!.Converter.ConvertToProvider(enumeration.Value),
                (TProviderType clrValue) => BaseEnumeration<TEnum, TValue>.ParseValue((TValue)relationalTypeMapping!.Converter.ConvertFromProvider(clrValue))
            );
        }
    }
}