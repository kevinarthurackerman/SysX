﻿namespace Sysx;

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

    /// <summary>
    /// Returns if the given type is assignable to a generic type.
    /// </summary>
    public static bool IsAssignableToGenericType(this Type type, Type genericType)
    {
        EnsureArg.IsNotNull(type);
        EnsureArg.IsNotNull(genericType);

        var interfaceTypes = type.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
            return true;

        Type baseType = type.BaseType;
        if (baseType == null) return false;

        return IsAssignableToGenericType(baseType, genericType);
    }

    /// <inheritdoc cref="GetGenericTypeArgument(Type, Type, int)"/>
    public static Type? GetGenericTypeArgument(this Type type, Type genericType) =>
        GetGenericTypeArgument(type, genericType, 0);

    /// <summary>
    /// Gets a generic type argument for a type implementing or inheriting from a generic type.
    /// Returns null if the generic type is not implemented or inherited from.
    /// </summary>
    public static Type? GetGenericTypeArgument(this Type type, Type genericType, int argumentIndex)
    {
        EnsureArg.IsNotNull(type);
        EnsureArg.IsNotNull(genericType);
        EnsureArg.IsTrue(genericType.IsGenericType, optsFn: x => x.WithMessage($"Type of {nameof(genericType)} must be a generic type"));
        EnsureArg.IsGte(argumentIndex, 0);
        EnsureArg.IsTrue(
            genericType.GetGenericArguments().Length > argumentIndex,
            optsFn: x => x.WithMessage($"Generic type argument index {nameof(argumentIndex)} {argumentIndex} is out of bounds for {nameof(genericType)} {genericType}."));

        var interfaceTypes = type.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
            {
                return it.GetGenericArguments()[argumentIndex];
            }
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
        {
            return type.GetGenericArguments()[argumentIndex];
        }

        Type baseType = type.BaseType;
        if (baseType == null) return null;

        return GetGenericTypeArgument(baseType, genericType, argumentIndex);
    }
}