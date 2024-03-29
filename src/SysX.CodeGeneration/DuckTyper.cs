﻿namespace SysX.CodeGeneration;

/// <inheritdoc cref="DuckTyper{TValue, TWithInterface}" />
public static class DuckTyper
{
	private static readonly ConcurrentDictionary<CacheKey, WrapMethod> _wrapCache = new();
	private static readonly ConcurrentDictionary<CacheKey, TryWrapMethod> _tryWrapCache = new();

	/// <inheritdoc cref="DuckTyper{TValue, TWithInterface}.Wrap(TValue, bool)" />
	public static TWithInterface Wrap<TWithInterface>(object? value) =>
		Wrap<TWithInterface>(value, includePrivateMembers: false);

	/// <inheritdoc cref="DuckTyper{TValue, TWithInterface}.Wrap(TValue, bool)" />
	public static TWithInterface Wrap<TWithInterface>(object? value, bool includePrivateMembers)
	{
		EnsureArg.IsTrue(typeof(TWithInterface).IsInterface,
			nameof(TWithInterface),
			x => x.WithMessage($"{nameof(TWithInterface)} must be an interface type."));
		EnsureArg.HasValue(value, nameof(value));

		var key = new CacheKey(value.GetType(), typeof(TWithInterface), includePrivateMembers);
		var duckTyper = _wrapCache.GetOrAdd(key,
			key =>
			{
				var wrapMethod = typeof(DuckTyper<,>)
					.MakeGenericType(key.Value, key.WithInterface)
					.GetMethod(nameof(DuckTyper<object, object>.Wrap), new[] { key.Value, typeof(bool) })!;

				return value => wrapMethod.Invoke(null, new[] { value, key.IncludePrivateMembers })!;
			});

		return (TWithInterface)duckTyper(value);
	}

	/// <inheritdoc cref="DuckTyper{TValue, TWithInterface}.TryWrap(TValue, out TWithInterface, bool)" />
	public static bool TryWrap<TWithInterface>(object? value, out TWithInterface? withInterface) =>
		TryWrap(value, out withInterface, includePrivateMembers: false);

	/// <inheritdoc cref="DuckTyper{TValue, TWithInterface}.TryWrap(TValue, out TWithInterface, bool)" />
	public static bool TryWrap<TWithInterface>(object? value, out TWithInterface? withInterface, bool includePrivateMembers)
	{
		if (!typeof(TWithInterface).IsInterface && value != null)
		{
			withInterface = default;
			return false;
		}

		var key = new CacheKey(value!.GetType(), typeof(TWithInterface), includePrivateMembers);
		var duckTyper = _tryWrapCache.GetOrAdd(key,
			key =>
			{
				var tryWrapMethod = typeof(DuckTyper<,>)
					.MakeGenericType(key.Value, key.WithInterface)
					.GetMethod(nameof(DuckTyper<object, object>.TryWrap),
						new[] { key.Value, key.WithInterface.MakeByRefType(), typeof(bool) })!;

				return (object value, out object? withInterface) =>
				{
					var parameters = new[] { value, null, key.IncludePrivateMembers };
					var success = (bool)tryWrapMethod.Invoke(null, parameters)!;
					withInterface = parameters[1];
					return success;
				};
			});

		var success = duckTyper(value, out var result);

		withInterface = result == null ? default : (TWithInterface)result;
		return success;
	}

	private record struct CacheKey(Type Value, Type WithInterface, bool IncludePrivateMembers);
	private delegate object WrapMethod(object value);
	private delegate bool TryWrapMethod(object value, out object? withInterface);
}

/// <summary>
/// Wraps values with <see langword="interface"/>s so that they can be used in a duck-typed way.
/// By including <see langword="private"/> members the <see langword="interface"/> wrapper can also be used to access the <see cref="Type"/> as a "friended" <see langword="class"/>.
/// </summary>
public static class DuckTyper<TValue, TWithInterface>
{
	private static readonly Type? publicMemberWrapper;
	private static readonly bool? publicMembersFullyWrapped;

	private static readonly Type? publicAndPrivateMemberWrapper;
	private static readonly bool? publicAndPrivateMembersFullyWrapped;

	static DuckTyper()
	{
		if (!typeof(TWithInterface).IsInterface) return;

		var scriptOptions = ScriptOptions.Default
			.AddReferences(Assembly.GetAssembly(typeof(Type)))
			.AddReferences(Assembly.GetAssembly(typeof(TValue)))
			.AddReferences(Assembly.GetAssembly(typeof(TWithInterface)));

		var publicWrapperResult = GenerateWrapperCode(false);

		var publicWrapperScript = CSharpScript.Create(publicWrapperResult.Code, scriptOptions);
		publicWrapperScript.Compile();

		publicMemberWrapper = (Type)publicWrapperScript.RunAsync().Result.ReturnValue;

		publicMembersFullyWrapped = publicWrapperResult.FullyMapped;

		var publicAndPrivateWrapperResult = GenerateWrapperCode(true);

		var publicAndPrivateWrapperScript = CSharpScript.Create(publicAndPrivateWrapperResult.Code, scriptOptions);
		publicAndPrivateWrapperScript.Compile();

		publicAndPrivateMemberWrapper = (Type)publicAndPrivateWrapperScript.RunAsync().Result.ReturnValue;

		publicAndPrivateMembersFullyWrapped = publicAndPrivateWrapperResult.FullyMapped;
	}

	/// <inheritdoc cref="DuckTyper{TValue, TWithInterface}.Wrap(TValue, bool)" />
	public static TWithInterface Wrap(TValue value) =>
		Wrap(value, includePrivateMembers: false);

	/// <summary>
	/// Wraps values with <see langword="interface"/>s so that they can be used in a duck-typed way.
	/// By including <see langword="private"/> members the <see langword="interface"/> wrapper can also be used to access the <see cref="Type"/> as a "friended" <see langword="class"/>.
	/// Members that cannot be mapped to the target value will result in an <see cref="InvalidOperationException"/> when called.
	/// </summary>
	public static TWithInterface Wrap(TValue value, bool includePrivateMembers)
	{
		EnsureArg.IsTrue(typeof(TWithInterface).IsInterface,
			nameof(TWithInterface),
			x => x.WithMessage($"{nameof(TWithInterface)} must be an interface type."));
		EnsureArg.HasValue(value, nameof(value));

		if (includePrivateMembers)
		{
			return (TWithInterface)Activator.CreateInstance(publicAndPrivateMemberWrapper!, value)!;
		}

		return (TWithInterface)Activator.CreateInstance(publicMemberWrapper!, value)!;
	}

	/// <inheritdoc cref="DuckTyper{TValue, TWithInterface}.TryWrap(TValue, out TWithInterface, bool)" />
	public static bool TryWrap(TValue value, out TWithInterface? withInterface) =>
		TryWrap(value, out withInterface, includePrivateMembers: false);

	/// <summary>
	/// Tries to wrap the target value with an <see langword="interface"/> so that it can be used in a duck-typed way.
	/// By including <see langword="private"/> members the <see langword="interface"/> wrapper can also be used to access the <see cref="Type"/> as a "friended" <see langword="class"/>.
	/// Will only return <see langword="true"/> when all members of the target value can be wrapped by the <see langword="interface"/>.
	/// </summary>
	public static bool TryWrap(TValue value, out TWithInterface? withInterface, bool includePrivateMembers)
	{
		if (typeof(TWithInterface).IsInterface && value != null)
		{
			if (includePrivateMembers)
			{
				if (publicAndPrivateMembersFullyWrapped!.Value)
				{
					withInterface = (TWithInterface)Activator.CreateInstance(publicAndPrivateMemberWrapper!, value)!;
					return true;
				}
			}
			else
			{
				if (publicMembersFullyWrapped!.Value)
				{
					withInterface = (TWithInterface)Activator.CreateInstance(publicMemberWrapper!, value)!;
					return true;
				}
			}
		}

		withInterface = default;
		return false;
	}

	private static GenerateWrapperCodeResult GenerateWrapperCode(bool includePrivateMembers)
	{
		var valueMemberBindingFlags = FlagsEnum.Combine(BindingFlags.Public, BindingFlags.Instance);
		if (includePrivateMembers) valueMemberBindingFlags = FlagsEnum.Add(valueMemberBindingFlags, BindingFlags.NonPublic);

		var valueFields = typeof(TValue).GetFields(valueMemberBindingFlags);

		var valueProperties = typeof(TValue).GetProperties(valueMemberBindingFlags)
			.Where(x => includePrivateMembers || (x.GetGetMethod()?.IsPublic ?? false) || (x.GetSetMethod()?.IsPublic ?? false))
			.ToArray();

		var valueMethods = typeof(TValue).GetMethods(valueMemberBindingFlags);

		var interfaceMemberBindingFlags = FlagsEnum.Combine(BindingFlags.Public, BindingFlags.Instance);

		var interfaceProperties = typeof(TWithInterface).GetProperties(interfaceMemberBindingFlags);
		var interfaceMethods = typeof(TWithInterface).GetMethods(interfaceMemberBindingFlags)
			.Where(x => !x.IsSpecialName)
			.ToArray();

		var interfaceType = typeof(TWithInterface).GetIdentifier();
		var valueType = typeof(TValue).GetIdentifier();
		var wrapperType = $"{typeof(TWithInterface).Name}_Wraping_{typeof(TValue).Name}_Public{(includePrivateMembers ? "_And_Private" : "")}_Members";
		var invalidOperationExceptionType = typeof(InvalidOperationException).GetIdentifier();
		var argumentNullExceptionType = typeof(ArgumentNullException).GetIdentifier();
		var type = typeof(Type).GetIdentifier();
		var array = typeof(Array).GetIdentifier();
		var @object = typeof(object).GetIdentifier();

		var fullyMapped = true;
		var cachedMemberNumber = 0;

		var staticPrivateFields = new StringBuilder();
		var publicMembers = new StringBuilder();

		var innerValue = "innerValue";
		var fieldInfo = typeof(FieldInfo).GetIdentifier();
		var methodInfo = typeof(MethodInfo).GetIdentifier();
		var bindingFlags = typeof(BindingFlags).GetIdentifier();
		var nonPublicInstanceBindingFlags = $"{bindingFlags}.{nameof(BindingFlags.NonPublic)} | {bindingFlags}.{nameof(BindingFlags.Instance)}";

		foreach (var interfaceProperty in interfaceProperties)
		{
			var interfacePropertyName = interfaceProperty.Name;
			var interfacePropertyType = interfaceProperty.PropertyType.GetIdentifier();
			var interfacePropertySignature = $"public {interfacePropertyType} {interfacePropertyName}";

			var valueFieldInfo = valueFields.SingleOrDefault(x =>
				x.Name == interfaceProperty.Name && x.FieldType == interfaceProperty.PropertyType);

			var valuePropertyInfo = valueProperties.SingleOrDefault(x =>
				x.Name == interfaceProperty.Name && x.PropertyType == interfaceProperty.PropertyType);

			var valueMethodInfo = valueMethods.SingleOrDefault(x =>
				x.Name == interfaceProperty.Name && x.ReturnType == interfaceProperty.PropertyType);

			var interfacePropertyHasGet = interfaceProperty.GetGetMethod() != null;
			var interfacePropertyHasSet = interfaceProperty.GetSetMethod() != null;

			publicMembers.AppendLine();
			publicMembers.AppendLine($@"	{interfacePropertySignature}");
			publicMembers.AppendLine($@"	{{");

			var matchCount = 0;
			if (valueFieldInfo != null) matchCount++;
			if (valuePropertyInfo != null) matchCount++;
			if (valueMethodInfo != null) matchCount++;

			if (matchCount == 0)
			{
				// map to exception, no mapping found
				fullyMapped = false;

				if (interfacePropertyHasGet)
				{
					publicMembers.AppendLine($@"		get => throw new {invalidOperationExceptionType}(""No accessible field or property {interfaceType}.{interfacePropertyName} => {interfacePropertyType} found on wrapped value {valueType}."");");
				}
				if (interfacePropertyHasSet)
				{
					publicMembers.AppendLine($@"		set => throw new {invalidOperationExceptionType}(""No accessible field or property {interfaceType}.{interfacePropertyName} => {interfacePropertyType} found on wrapped value {valueType}."");");
				}
			}
			else if (matchCount > 1)
			{
				// map to exception, multiple mappings found
				fullyMapped = false;

				if (interfacePropertyHasGet)
				{
					publicMembers.AppendLine($@"		get => throw new {invalidOperationExceptionType}(""Multiple members matching {interfaceType}.{interfacePropertyName} => {interfacePropertyType} found on wrapped value {valueType}."");");
				}
				if (interfacePropertyHasSet)
				{
					publicMembers.AppendLine($@"		set => throw new {invalidOperationExceptionType}(""Multiple members matching {interfaceType}.{interfacePropertyName} => {interfacePropertyType} found on wrapped value {valueType}."");");
				}
			}
			else if (valueFieldInfo != null)
			{
				// map to field
				var valueField = valueFieldInfo.Name;

				if (valueFieldInfo.IsPublic)
				{
					if (interfacePropertyHasGet)
					{
						publicMembers.AppendLine($@"		get => {innerValue}.{valueField};");
					}
					if (interfacePropertyHasSet)
					{
						if (!valueFieldInfo.IsInitOnly)
						{
							publicMembers.AppendLine($@"		set => {innerValue}.{valueField} = value;");
						}
						else
						{
							fullyMapped = false;

							publicMembers.AppendLine($@"		set => throw new {invalidOperationExceptionType}(""The field matching {interfaceType}.{interfacePropertyName} => {interfacePropertyType} found on wrapped value {valueType} is init only."");");
						}
					}
				}
				else
				{
					var staticValueFieldInfo = $@"{valueField}_FieldInfo_{cachedMemberNumber++}";

					staticPrivateFields.AppendLine($@"	private static readonly {fieldInfo} {staticValueFieldInfo} = typeof({valueType}).GetField(""{valueField}"", {nonPublicInstanceBindingFlags});");

					if (interfacePropertyHasGet)
					{
						publicMembers.AppendLine($@"		get => ({interfacePropertyType}){staticValueFieldInfo}.GetValue({innerValue});");
					}
					if (interfacePropertyHasSet)
					{
						if (!valueFieldInfo.IsInitOnly)
						{
							publicMembers.AppendLine($@"		set => {staticValueFieldInfo}.SetValue({innerValue}, value);");
						}
						else
						{
							fullyMapped = false;

							publicMembers.AppendLine($@"		set => throw new {invalidOperationExceptionType}(""The field matching {interfaceType}.{interfacePropertyName} => {interfacePropertyType} found on wrapped value {valueType} is init only."");");
						}
					}

				}
			}
			else if (valuePropertyInfo != null)
			{
				// map to property
				var valueProperty = valuePropertyInfo.Name;

				var valuePropertyGet = valuePropertyInfo.GetGetMethod(includePrivateMembers);
				var valuePropertySet = valuePropertyInfo.GetSetMethod(includePrivateMembers);

				if (interfacePropertyHasGet)
				{
					if (valuePropertyGet == null)
					{
						fullyMapped = false;

						publicMembers.AppendLine($@"		get => throw new {invalidOperationExceptionType}(""The property matching {interfaceType}.{interfacePropertyName} => {interfacePropertyType} found on wrapped value {valueType} is has no accessible get method defined."");");
					}
					else if (valuePropertyGet.IsPublic)
					{
						publicMembers.AppendLine($@"		get => {innerValue}.{valueProperty};");
					}
					else
					{
						var staticValuePropertyGetMethodInfo = $@"get_{valueProperty}_MethodInfo_{cachedMemberNumber++}";

						staticPrivateFields.AppendLine($@"	private static readonly {methodInfo} {staticValuePropertyGetMethodInfo} = typeof({valueType}).GetProperty(""{valueProperty}"", {nonPublicInstanceBindingFlags}).GetGetMethod(true);");

						publicMembers.AppendLine($@"		get => ({interfacePropertyType}){staticValuePropertyGetMethodInfo}.Invoke({innerValue}, null);");
					}
				}

				if (interfacePropertyHasSet)
				{
					if (valuePropertySet == null)
					{
						fullyMapped = false;

						publicMembers.AppendLine($@"		set => throw new {invalidOperationExceptionType}(""The property matching {interfaceType}.{interfacePropertyName} => {interfacePropertyType} found wrapped value {valueType} is has no accessible set method defined."");");
					}
					else if (valuePropertySet.IsPublic)
					{
						publicMembers.AppendLine($@"		set => {innerValue}!.{valueProperty} = value;");
					}
					else
					{
						var staticValuePropertySetMethodInfo = $@"Set_{valueProperty}_MethodInfo_{cachedMemberNumber++}";

						staticPrivateFields.AppendLine($@"	private static readonly {methodInfo} {staticValuePropertySetMethodInfo} = typeof({valueType}).GetProperty(""{valueProperty}"", {nonPublicInstanceBindingFlags}).GetSetMethod(true);");

						publicMembers.AppendLine($@"		set => {staticValuePropertySetMethodInfo}.Invoke({innerValue}, new {@object}[] {{ value }});");
					}
				}
			}
			else if (valueMethodInfo != null)
			{
				// map to exception, invalid mapping to method
				fullyMapped = false;

				if (interfacePropertyHasGet)
				{
					publicMembers.AppendLine($@"		get => throw new {invalidOperationExceptionType}(""Cannot map method to {interfaceType}.{interfacePropertyName} => {interfacePropertyType} on wrapped value {valueType}, a method must map to a method."");");
				}
				if (interfacePropertyHasSet)
				{
					publicMembers.AppendLine($@"		set => throw new {invalidOperationExceptionType}(""Cannot map method to {interfaceType}.{interfacePropertyName} => {interfacePropertyType} on wrapped value {valueType}, a method must map to a method."");");
				}
			}
			else
			{
				// this block should be unreachable
				throw new Exception("Else block was reached unexpectedly.");
			}

			publicMembers.AppendLine($@"	}}");
		}

		foreach (var interfaceMethodInfo in interfaceMethods)
		{
			var interfaceMethod = interfaceMethodInfo.Name;
			var interfaceMethodReturnType = interfaceMethodInfo.ReturnType.GetIdentifier();
			var interfaceMethodParameterList = string.Join(", ",
				interfaceMethodInfo.GetParameters().Select(x => $"{(x.ParameterType.IsByRef ? "out " : string.Empty)}{x.ParameterType.GetIdentifier()} {x.Name}"));
			var interfaceMethodParameterNameList = string.Join(", ",
				interfaceMethodInfo.GetParameters().Select(x => $"{(x.ParameterType.IsByRef ? "out " : string.Empty)}{x.Name}"));
			var interfaceMethodParameterTypeList = string.Join(", ",
				interfaceMethodInfo.GetParameters().Select(x => $"{(x.ParameterType.IsByRef ? "out " : string.Empty)}{x.ParameterType.GetIdentifier()}"));
			var interfaceMethodTypeIdentifiersList = string.Join(", ",
				interfaceMethodInfo.GetParameters().Select(x => $"typeof({x.ParameterType.GetIdentifier()}){(x.ParameterType.IsByRef ? ".MakeByRefType()" : string.Empty)}"));
			var interfaceMethodSignature = $"public {interfaceMethodReturnType} {interfaceMethod}({interfaceMethodParameterList})";

			var valueFieldInfo = valueFields.SingleOrDefault(x =>
				x.Name == interfaceMethodInfo.Name && x.FieldType == interfaceMethodInfo.ReturnType);

			var valuePropertyInfo = valueProperties.SingleOrDefault(x =>
				x.Name == interfaceMethodInfo.Name && x.PropertyType == interfaceMethodInfo.ReturnType);

			var valueMethodInfo = valueMethods.SingleOrDefault(x => x.MatchesSignature(interfaceMethodInfo));

			publicMembers.AppendLine();

			var matchCount = 0;
			if (valueFieldInfo != null) matchCount++;
			if (valuePropertyInfo != null) matchCount++;
			if (valueMethodInfo != null) matchCount++;

			if (matchCount == 0)
			{
				// map to exception, no mapping found
				fullyMapped = false;

				publicMembers.AppendLine($@"	{interfaceMethodSignature} => throw new {invalidOperationExceptionType}(""No accessible method {interfaceType}.{interfaceMethod}({interfaceMethodParameterTypeList}) => {interfaceMethodReturnType} found on wrapped value {valueType}."");");
			}
			else if (matchCount > 1)
			{
				// map to exception, multiple mappings found
				fullyMapped = false;

				publicMembers.AppendLine($@"	{interfaceMethodSignature} => throw new {invalidOperationExceptionType}(""Multiple members matching {interfaceType}.{interfaceMethod}({interfaceMethodParameterTypeList}) => {interfaceMethodReturnType} found on wrapped value {valueType}."");");
			}
			else if (valueFieldInfo != null)
			{
				// map to exception, invalid mapping
				fullyMapped = false;

				publicMembers.AppendLine($@"	{interfaceMethodSignature} => throw new {invalidOperationExceptionType}(""Cannot map field to {interfaceType}.{interfaceMethod}({interfaceMethodParameterTypeList}) => {interfaceMethodReturnType} on wrapped value {valueType}, a field must map to a property."");");
			}
			else if (valuePropertyInfo != null)
			{
				// map to exception, invalid mapping
				fullyMapped = false;

				publicMembers.AppendLine($@"	{interfaceMethodSignature} => throw new {invalidOperationExceptionType}(""Cannot map property to {interfaceType}.{interfaceMethod}({interfaceMethodParameterTypeList}) => {interfaceMethodReturnType} on wrapped value {valueType}, a property must map to a property."");");
			}
			else if (valueMethodInfo != null)
			{
				// map to method
				if (valueMethodInfo.IsPublic)
				{
					publicMembers.AppendLine($@"	{interfaceMethodSignature} => innerValue.{interfaceMethod}({interfaceMethodParameterNameList});");
				}
				else
				{
					var staticValueMethodInfo = $@"{interfaceMethod}_MethodInfo_{cachedMemberNumber++}";
					var parameters = interfaceMethodInfo.GetParameters();

					staticPrivateFields.AppendLine($@"	private static readonly {methodInfo} {staticValueMethodInfo} = typeof({valueType}).GetMethod(""{interfaceMethod}"", {nonPublicInstanceBindingFlags}, null, new {type}[]{{ {interfaceMethodTypeIdentifiersList} }}, null);");

					publicMembers.AppendLine($@"	{interfaceMethodSignature}");
					publicMembers.AppendLine($@"	{{");
					if (parameters.Any())
					{
						publicMembers.AppendLine($@"		var __parameters = new {@object}[]");
						publicMembers.AppendLine($@"		{{");
						for (var i = 0; i < parameters.Length; i++)
						{
							if (parameters[i].IsOut)
							{
								publicMembers.Append($@"			default({parameters[i].ParameterType.GetIdentifier()})");
							}
							else
							{
								publicMembers.Append($@"			{parameters[i].Name}");
							}
							publicMembers.AppendLine(parameters[i] != parameters.Last() ? "," : string.Empty);
						}
						publicMembers.AppendLine($@"		}};");
					}
					else
					{
						publicMembers.AppendLine($@"		var __parameters = {array}.{nameof(Array.Empty)}<{@object}>()");
					}
					publicMembers.AppendLine($@"		var __result = ({interfaceMethodReturnType}){staticValueMethodInfo}.Invoke({innerValue}, __parameters);");
					for (var i = 0; i < parameters.Length; i++)
					{
						if (parameters[i].IsOut)
						{
							publicMembers.AppendLine($@"		{parameters[i].Name} = ({parameters[i].ParameterType.GetIdentifier()})__parameters[{i}];");
						}
					}
					publicMembers.AppendLine($@"		return __result;");
					publicMembers.AppendLine($@"	}}");
				}
			}
			else
			{
				// this block should be unreachable
				throw new Exception("Else block was reached unexpectedly.");
			}
		}

		var codeBuilder = new StringBuilder();
		codeBuilder.AppendLine($@"#nullable disable");
		codeBuilder.AppendLine($@"internal class {wrapperType}: {interfaceType}");
		codeBuilder.AppendLine($@"{{");
		if (staticPrivateFields.Length > 0)
		{
			codeBuilder.Append(staticPrivateFields.ToString());
			codeBuilder.AppendLine();
		}
		codeBuilder.AppendLine($@"	private readonly {valueType} {innerValue};");
		codeBuilder.AppendLine();
		codeBuilder.AppendLine($@"	public {wrapperType}({valueType} valueToWrap)");
		codeBuilder.AppendLine($@"	{{");
		codeBuilder.AppendLine($@"		{innerValue} = valueToWrap ?? throw new {argumentNullExceptionType}(nameof(valueToWrap));");
		codeBuilder.AppendLine($@"	}}");
		if (publicMembers.Length > 0)
		{
			codeBuilder.Append(publicMembers.ToString());
		}
		codeBuilder.AppendLine($@"}}");
		codeBuilder.AppendLine();
		codeBuilder.Append($@"return typeof({wrapperType});");

		return new GenerateWrapperCodeResult(codeBuilder.ToString(), fullyMapped);
	}

	private record struct GenerateWrapperCodeResult(string Code, bool FullyMapped);
}