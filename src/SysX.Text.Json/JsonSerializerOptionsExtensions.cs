namespace SysX.Text.Json;

public static class JsonSerializerOptionsExtensions
{
	/// <summary>
	/// Registers converters for identifier types.
	/// </summary>
	public static JsonSerializerOptions UseIdentifiers(this JsonSerializerOptions jsonSerializerOptions)
	{
		EnsureArg.IsNotNull(jsonSerializerOptions, nameof(jsonSerializerOptions));

		jsonSerializerOptions.Converters.Add(new DelegatingJsonConverter<BinaryGuid, Guid>());
		jsonSerializerOptions.Converters.Add(new DelegatingJsonConverter<SqlServerGuid, Guid>());
		jsonSerializerOptions.Converters.Add(new DelegatingJsonConverter<StringGuid, Guid>());

		return jsonSerializerOptions;
	}

	/// <inheritdoc cref="UseEnumerationsByDisplayName(JsonSerializerOptions, ReflectionSource)" />
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static JsonSerializerOptions UseEnumerationsByDisplayName(this JsonSerializerOptions jsonSerializerOptions) =>
		jsonSerializerOptions.UseEnumerationsByDisplayName(ReflectionSource.GetCallingSource());

	/// <summary>
	/// Registers converters for types extending BaseEnumeration.
	/// </summary>
	public static JsonSerializerOptions UseEnumerationsByDisplayName(this JsonSerializerOptions jsonSerializerOptions, ReflectionSource reflectionSource)
	{
		EnsureArg.IsNotNull(jsonSerializerOptions, nameof(jsonSerializerOptions));
		EnsureArg.IsNotNull(reflectionSource, nameof(reflectionSource));

		foreach (var type in reflectionSource)
		{
			var baseEnumerationType = GetBaseEnumerationType(type);

			if (baseEnumerationType == null) continue;

			var genericParams = baseEnumerationType.GetGenericArguments();
			var enumType = genericParams[0];
			var valueType = genericParams[1];

			var jsonConverter = (JsonConverter)typeof(JsonSerializerOptionsExtensions)
				.GetMethod(nameof(CreateEnumerationByNameConverter), BindingFlags.Static | BindingFlags.NonPublic)!
				.MakeGenericMethod(type, enumType, valueType)
				.Invoke(null, null)!;

			jsonSerializerOptions.Converters.Add(jsonConverter);
		}

		return jsonSerializerOptions;
	}

	/// <inheritdoc cref="UseEnumerationsByValue(JsonSerializerOptions, ReflectionSource)" />
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static JsonSerializerOptions UseEnumerationsByValue(this JsonSerializerOptions jsonSerializerOptions) =>
		jsonSerializerOptions.UseEnumerationsByValue(ReflectionSource.GetCallingSource());

	/// <summary>
	/// Registers converters for types extending BaseEnumeration.
	/// </summary>
	public static JsonSerializerOptions UseEnumerationsByValue(this JsonSerializerOptions jsonSerializerOptions, ReflectionSource reflectionSource)
	{
		EnsureArg.IsNotNull(jsonSerializerOptions, nameof(jsonSerializerOptions));
		EnsureArg.IsNotNull(reflectionSource, nameof(reflectionSource));

		foreach (var type in reflectionSource)
		{
			var baseEnumerationType = GetBaseEnumerationType(type);

			if (baseEnumerationType == null) continue;

			var genericParams = baseEnumerationType.GetGenericArguments();
			var enumType = genericParams[0];
			var valueType = genericParams[1];

			var jsonConverter = (JsonConverter)typeof(JsonSerializerOptionsExtensions)
				.GetMethod(nameof(CreateEnumerationByValueConverter), BindingFlags.Static | BindingFlags.NonPublic)!
				.MakeGenericMethod(type, enumType, valueType)
				.Invoke(null, null)!;

			jsonSerializerOptions.Converters.Add(jsonConverter);
		}

		return jsonSerializerOptions;
	}

	private static JsonConverter CreateEnumerationByNameConverter<TEnumeration, TEnum, TValue>()
		where TEnumeration : TEnum
		where TEnum : BaseEnumeration<TEnum, TValue>
		where TValue : IComparable<TValue>, IEquatable<TValue>, IComparable
	{
		return new DelegatingJsonConverter<TEnumeration, string>(
			x => x?.DisplayName,
			x => (TEnumeration)BaseEnumeration<TEnum, TValue>.Parse(x)
		);
	}

	private static JsonConverter CreateEnumerationByValueConverter<TEnumeration, TEnum, TValue>()
		where TEnumeration : TEnum
		where TEnum : BaseEnumeration<TEnum, TValue>
		where TValue : IComparable<TValue>, IEquatable<TValue>, IComparable
	{
		return new DelegatingJsonConverter<TEnumeration, TValue>(
			x => x == null ? default : x.Value,
			x => (TEnumeration)BaseEnumeration<TEnum, TValue>.ParseValue(x)
		);
	}

	private static Type? GetBaseEnumerationType(Type type)
	{
		var baseType = type!;

		while (baseType != null && baseType != baseType.BaseType)
		{
			if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(BaseEnumeration<,>))
			{
				return baseType;
			}

			baseType = baseType.BaseType;
		}

		return null;
	}
}
