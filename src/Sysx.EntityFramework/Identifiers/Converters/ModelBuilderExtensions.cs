namespace Sysx.EntityFramework.Identifiers.Converters;

public static class ModelBuilderExtensions
{
    public static ModelBuilder RegisterSequentialGuidConversions(this ModelBuilder modelBuilder) =>
        modelBuilder.RegisterBinaryGuidConversions()
            .RegisterStringGuidConversions()
            .RegisterSqlServerGuidConversions();

    public static ModelBuilder RegisterBinaryGuidConversions(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach(var property in entity.ClrType.GetProperties())
            {
                if (property.PropertyType != typeof(BinaryGuid)) continue;

                var entityProp = entity.AddProperty(property);
                entityProp.SetProviderClrType(typeof(byte[]));
                entityProp.SetMaxLength(16);
                entityProp.SetIsFixedLength(true);
                entityProp.SetValueConverter(ValueConverters.BinaryGuid);
            }
        }

        return modelBuilder;
    }

    public static ModelBuilder RegisterStringGuidConversions(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.ClrType.GetProperties())
            {
                if (property.PropertyType != typeof(StringGuid)) continue;

                var entityProp = entity.AddProperty(property);

                entityProp.SetProviderClrType(typeof(string));
                entityProp.SetMaxLength(36);
                entityProp.SetIsFixedLength(true);
                entityProp.SetIsUnicode(false);
                entityProp.SetValueConverter(ValueConverters.StringGuid);
            }
        }

        return modelBuilder;
    }

    public static ModelBuilder RegisterSqlServerGuidConversions(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.ClrType.GetProperties())
            {
                if (property.PropertyType != typeof(SqlServerGuid)) continue;

                var entityProp = entity.AddProperty(property);
                entityProp.SetProviderClrType(typeof(Guid));
                entityProp.SetValueConverter(ValueConverters.SqlServerGuid);
            }
        }

        return modelBuilder;
    }
}