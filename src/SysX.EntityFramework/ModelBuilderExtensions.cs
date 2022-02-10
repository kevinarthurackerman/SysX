namespace SysX.EntityFramework;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Configures all entity properties matching the type.
    /// </summary>
    public static ModelBuilder RegisterPropertiesOfType<TValue>(this ModelBuilder modelBuilder, Action<IMutableProperty> configureValue)
    {
        EnsureArg.IsNotNull(modelBuilder, nameof(modelBuilder));
        EnsureArg.IsNotNull(configureValue, nameof(configureValue));

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