namespace Sysx.Text.Json;

public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Registers converters for identifier types.
    /// </summary>
    public static JsonSerializerOptions UseIdentifiers(this JsonSerializerOptions jsonSerializerOptions)
    {
        EnsureArg.IsNotNull(jsonSerializerOptions, nameof(jsonSerializerOptions));

        jsonSerializerOptions.Converters.Add(BinaryGuidJsonConverter.Instance);
        jsonSerializerOptions.Converters.Add(SqlServerGuidJsonConverter.Instance);
        jsonSerializerOptions.Converters.Add(StringGuidJsonConverter.Instance);

        return jsonSerializerOptions;
    }

    /// <inheritdoc cref="UseEnumerationsByDisplayName(JsonSerializerOptions, Assembly)" />
    public static JsonSerializerOptions UseEnumerationsByDisplayName(this JsonSerializerOptions jsonSerializerOptions) =>
        jsonSerializerOptions.UseEnumerationsByDisplayName(Assembly.GetCallingAssembly());

    /// <summary>
    /// Registers converters for types extending BaseEnumeration.
    /// </summary>
    public static JsonSerializerOptions UseEnumerationsByDisplayName(this JsonSerializerOptions jsonSerializerOptions, Assembly scanAssembly)
    {
        EnsureArg.IsNotNull(jsonSerializerOptions, nameof(jsonSerializerOptions));
        EnsureArg.IsNotNull(scanAssembly, nameof(scanAssembly));

        foreach (var type in scanAssembly.GetTypes())
        {
            var baseEnumerationType = GetBaseEnumerationType(type);

            if (baseEnumerationType == null) continue;

            var genericParams = baseEnumerationType.GetGenericArguments();
            var enumType = genericParams[0];
            var valueType = genericParams[1];

            var jsonConverter = (JsonConverter)typeof(BaseEnumerationByDisplayNameConverter<,,>)
                .MakeGenericType(type, enumType, valueType)
                .GetField("Instance", BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null)!;


            jsonSerializerOptions.Converters.Add(jsonConverter);
        }

        return jsonSerializerOptions;
    }

    /// <inheritdoc cref="UseEnumerationsByValue(JsonSerializerOptions, Assembly)" />
    public static JsonSerializerOptions UseEnumerationsByValue(this JsonSerializerOptions jsonSerializerOptions) =>
        jsonSerializerOptions.UseEnumerationsByValue(Assembly.GetCallingAssembly());

    /// <summary>
    /// Registers converters for types extending BaseEnumeration.
    /// </summary>
    public static JsonSerializerOptions UseEnumerationsByValue(this JsonSerializerOptions jsonSerializerOptions, Assembly scanAssembly)
    {
        EnsureArg.IsNotNull(jsonSerializerOptions, nameof(jsonSerializerOptions));
        EnsureArg.IsNotNull(scanAssembly, nameof(scanAssembly));

        foreach (var type in scanAssembly.GetTypes())
        {
            var baseEnumerationType = GetBaseEnumerationType(type);

            if (baseEnumerationType == null) continue;

            var genericParams = baseEnumerationType.GetGenericArguments();
            var enumType = genericParams[0];
            var valueType = genericParams[1];

            var jsonConverter = (JsonConverter)typeof(BaseEnumerationByValueConverter<,,>)
                .MakeGenericType(type, enumType, valueType)
                .GetField("Instance", BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null)!;

            jsonSerializerOptions.Converters.Add(jsonConverter);
        }

        return jsonSerializerOptions;
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
