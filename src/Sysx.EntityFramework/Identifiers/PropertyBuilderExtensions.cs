namespace Sysx.EntityFramework.Identifiers;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder IsBinaryGuid(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(byte[]))
            .HasMaxLength(16)
            .IsFixedLength(true)
            .HasConversion(ValueConverters.BinaryGuid);

    public static PropertyBuilder IsStringGuid(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(string))
            .HasMaxLength(36)
            .IsFixedLength(true)
            .IsUnicode(false)
            .HasConversion(ValueConverters.StringGuid);

    public static PropertyBuilder IsSqlServerGuid(this PropertyBuilder propertyBuilder) =>
        propertyBuilder.HasConversion(typeof(Guid))
            .HasConversion(ValueConverters.SqlServerGuid);
}