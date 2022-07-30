namespace SysX.Reflection;

public static class PropertyInfoExtensions
{
	private static readonly ConcurrentDictionary<PropertyInfo, FieldInfo?> getBackingFieldCache = new();

	public static FieldInfo? GetAutoBackingField(this PropertyInfo propertyInfo)
	{
		return getBackingFieldCache.GetOrAdd(propertyInfo, propertyInfo =>
		{
			var autoBackingFieldName = $"<{propertyInfo.Name}>k__BackingField";
			var autoBackingField = propertyInfo.DeclaringType!
				.GetField(autoBackingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

			return autoBackingField;
		});
	}

	public static void SetBackingValue<TValue>(this PropertyInfo propertyInfo, object target, TValue? value)
	{
		var autoBackingField = GetAutoBackingField(propertyInfo);

		if (autoBackingField == null)
			throw new InvalidOperationException("Property is not an auto-property");

		autoBackingField.SetValue(target, value);
	}
}
