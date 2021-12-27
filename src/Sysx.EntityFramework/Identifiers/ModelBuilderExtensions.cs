namespace Sysx.EntityFramework.Identifiers;

public static class ModelBuilderExtensions
{
    public static ModelBuilder RegisterSequentialGuidConversions(this ModelBuilder modelBuilder) =>
        modelBuilder.RegisterBinaryGuids()
            .RegisterStringGuids()
            .RegisterSqlServerGuids();

    public static ModelBuilder RegisterBinaryGuids(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(byte[]));
            entityProp.SetMaxLength(16);
            entityProp.SetIsFixedLength(true);
            entityProp.SetValueConverter(ValueConverters.BinaryGuid);
        };

        modelBuilder.RegisterPropertiesOfType<BinaryGuid>(registerType);
        modelBuilder.RegisterPropertiesOfType<BinaryGuid?>(registerType);

        return modelBuilder;
    }

    public static ModelBuilder RegisterStringGuids(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(string));
            entityProp.SetMaxLength(36);
            entityProp.SetIsFixedLength(true);
            entityProp.SetIsUnicode(false);
            entityProp.SetValueConverter(ValueConverters.StringGuid);
        };

        modelBuilder.RegisterPropertiesOfType<StringGuid>(registerType);
        modelBuilder.RegisterPropertiesOfType<StringGuid?>(registerType);

        return modelBuilder;
    }

    public static ModelBuilder RegisterSqlServerGuids(this ModelBuilder modelBuilder)
    {
        var registerType = (IMutableProperty entityProp) =>
        {
            entityProp.SetProviderClrType(typeof(Guid));
            entityProp.SetValueConverter(ValueConverters.SqlServerGuid);
        };

        modelBuilder.RegisterPropertiesOfType<SqlServerGuid>(registerType);
        modelBuilder.RegisterPropertiesOfType<SqlServerGuid?>(registerType);

        return modelBuilder;
    }
}