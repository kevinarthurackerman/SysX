namespace Sysx;

/// <summary>
/// Static <see langword="class"/> that provides extensions to <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    private static readonly IReadOnlyDictionary<Type, string> aliases = new ReadOnlyDictionary<Type, string>(new Dictionary<Type, string>()
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(object), "object" },
            { typeof(bool), "bool" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(void), "void" },
            { typeof(byte).MakeByRefType(), "byte" },
            { typeof(sbyte).MakeByRefType(), "sbyte" },
            { typeof(short).MakeByRefType(), "short" },
            { typeof(ushort).MakeByRefType(), "ushort" },
            { typeof(int).MakeByRefType(), "int" },
            { typeof(uint).MakeByRefType(), "uint" },
            { typeof(long).MakeByRefType(), "long" },
            { typeof(ulong).MakeByRefType(), "ulong" },
            { typeof(float).MakeByRefType(), "float" },
            { typeof(double).MakeByRefType(), "double" },
            { typeof(decimal).MakeByRefType(), "decimal" },
            { typeof(object).MakeByRefType(), "object" },
            { typeof(bool).MakeByRefType(), "bool" },
            { typeof(char).MakeByRefType(), "char" },
            { typeof(string).MakeByRefType(), "string" },
            { typeof(byte?), "byte?" },
            { typeof(sbyte?), "sbyte?" },
            { typeof(short?), "short?" },
            { typeof(ushort?), "ushort?" },
            { typeof(int?), "int?" },
            { typeof(uint?), "uint?" },
            { typeof(long?), "long?" },
            { typeof(ulong?), "ulong?" },
            { typeof(float?), "float?" },
            { typeof(double?), "double?" },
            { typeof(decimal?), "decimal?" },
            { typeof(bool?), "bool?" },
            { typeof(char?), "char?" },
            { typeof(byte?).MakeByRefType(), "byte?" },
            { typeof(sbyte?).MakeByRefType(), "sbyte?" },
            { typeof(short?).MakeByRefType(), "short?" },
            { typeof(ushort?).MakeByRefType(), "ushort?" },
            { typeof(int?).MakeByRefType(), "int?" },
            { typeof(uint?).MakeByRefType(), "uint?" },
            { typeof(long?).MakeByRefType(), "long?" },
            { typeof(ulong?).MakeByRefType(), "ulong?" },
            { typeof(float?).MakeByRefType(), "float?" },
            { typeof(double?).MakeByRefType(), "double?" },
            { typeof(decimal?).MakeByRefType(), "decimal?" },
            { typeof(bool?).MakeByRefType(), "bool?" },
            { typeof(char?).MakeByRefType(), "char?" }
        });

    /// <summary>
    /// Gets the code alias for the given <see cref="Type"/>.
    /// </summary>
    public static string? GetAlias(this Type? type)
    {
        if (type == null) return "null";
        aliases.TryGetValue(type, out var alias);
        return alias;
    }

    /// <summary>
    /// Get the identifier for the given <see cref="Type"/>. If the <see cref="Type"/> has an <see langword="alias"/> it is returned,
    /// otherwise the fully qualified name will be returned.
    /// </summary>
    public static string? GetIdentifier(this Type? type) =>
        GetAlias(type) ?? type!.FullName?.Replace('+', '.');

    /// <summary>
    /// Returns whether or not the given <see cref="Type"/> is <see langword="null"/>able.
    /// </summary>
    public static bool IsNullable(this Type type)
    {
        EnsureArg.IsNotNull(type, nameof(type));

        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }
}