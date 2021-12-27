namespace Sysx.EntityFramework;

public static class ModelBuilderExtensions
{
    public static ModelBuilder RegisterPropertiesOfType<TValue>(this ModelBuilder modelBuilder, Action<IMutableProperty> configureValue)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var propertyInfos = entityType.ClrType.GetProperties().Where(x => x.PropertyType == typeof(TValue));

            foreach (var propertyInfo in propertyInfos)
            {
                var entityProp = entityType.FindProperty(propertyInfo) ?? entityType.AddProperty(propertyInfo);

                configureValue(entityProp);
            }
        }

        return modelBuilder;
    }
}