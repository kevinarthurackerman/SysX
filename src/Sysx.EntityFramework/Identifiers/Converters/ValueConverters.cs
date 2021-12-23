namespace Sysx.EntityFramework.Identifiers.Converters;

internal static class ValueConverters
{
    internal static ValueConverter BinaryGuid =
        new ValueConverter<BinaryGuid, byte[]>(x => x.ToByteArray(), x => new BinaryGuid(new Guid(x)));

    internal static ValueConverter StringGuid =
        new ValueConverter<StringGuid, string>(x => x.ToString("D"), x => new StringGuid(Guid.ParseExact(x, "D")));

    internal static ValueConverter SqlServerGuid =
        new ValueConverter<SqlServerGuid, Guid>(x => x, x => x);
}
