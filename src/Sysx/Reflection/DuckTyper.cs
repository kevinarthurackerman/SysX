using EnsureThat;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sysx.Enums;

namespace Sysx.Reflection
{
    public class DuckTyper
    {
        private static readonly ConcurrentDictionary<CacheKey, WrapMethod> wrapCache = new();
        private static readonly ConcurrentDictionary<CacheKey, TryWrapMethod> tryWrapCache = new();

        internal static readonly ModuleBuilder DynamicModule;
        internal static readonly ConstructorInfo InvalidOperationExceptionCtor;

        static DuckTyper()
        {
            var assemblyName = new AssemblyName($"{nameof(DuckTyper)}_{Guid.NewGuid()}");
            
#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
            var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#endif
#if NET48
            var dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#endif
            var assembly = Assembly.GetEntryAssembly();

            DynamicModule = dynamicAssembly.DefineDynamicModule("core");

            InvalidOperationExceptionCtor = typeof(InvalidOperationException).GetConstructors()
                .Single(x =>
                {
                    if (!x.IsPublic) return false;

                    var parameters = x.GetParameters();

                    if (parameters.Length != 1) return false;
                    if (parameters[0].ParameterType != typeof(string)) return false;

                    return true;
                });
        }

        public static TWithInterface Wrap<TWithInterface>(object value)
        {
            EnsureArg.HasValue(value);

            var key = new CacheKey(value.GetType(), typeof(TWithInterface));

            var wrapMethod = wrapCache.GetOrAdd(key, x =>
            {
                var wrapMethod = typeof(DuckTyper<,>)
                    .MakeGenericType(x.ValueType, x.WithInterfaceType)
                    .GetMethod(nameof(DuckTyper<object, object>.Wrap))!;

                return value => wrapMethod.Invoke(null, new[] { value })!;
            });

            return (TWithInterface)wrapMethod(value);
        }

        public static bool TryWrap<TWithInterface>(object value, out TWithInterface? withInterface)
        {
            EnsureArg.HasValue(value);

            var key = new CacheKey(value.GetType(), typeof(TWithInterface));

            var wrapMethod = tryWrapCache.GetOrAdd(key, x =>
            {
                var tryWrapMethod = typeof(DuckTyper<,>)
                    .MakeGenericType(x.ValueType, x.WithInterfaceType)
                    .GetMethod(nameof(DuckTyper<object, object>.TryWrap))!;

                return (object value, out object? withInterface) =>
                {
                    var parameters = new[] { value, null };
                    var success = (bool)tryWrapMethod.Invoke(null, parameters)!;

                    if (success)
                    {
                        withInterface = parameters[1];
                        return true;
                    }

                    withInterface = default;
                    return false;
                };
            });

            if (wrapMethod(value, out var tryResult))
            {
                withInterface = (TWithInterface)tryResult!;
                return true;
            }

            withInterface = default;
            return false;
        }

        public static TWithInterface Wrap<TValue, TWithInterface>(TValue value) =>
            DuckTyper<TValue, TWithInterface>.Wrap(value);

        private record struct CacheKey(Type ValueType, Type WithInterfaceType);
        private delegate object WrapMethod(object value);
        private delegate bool TryWrapMethod(object value, out object? withInterface);
    }

    public static class DuckTyper<TValue, TWithInterface>
    {
        private static readonly ConstructorInfo wrapperCtor;

        public static bool CompletelyMapped { get; }

        static DuckTyper()
        {
            var wrapperType = CreateType(DuckTyper.DynamicModule);

            var innerValueField = CreateValueField(wrapperType);

            CreateConstructor(wrapperType, innerValueField);

            var handledTypes = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;
            var interfaceMembers = typeof(TWithInterface)
                .GetMembers()
                .Where(x => FlagsEnum.HasAny(x.MemberType, handledTypes))
                // remove get_### and set_### methods used to access properties and fields
                .Where(x => x is not MethodInfo xMethod || !xMethod.IsSpecialName)
                .ToArray();

            CompletelyMapped = true;

            foreach (var interfaceMember in interfaceMembers)
                CompletelyMapped &= HandleMember(wrapperType, innerValueField, interfaceMember);

            wrapperType.AddInterfaceImplementation(typeof(TWithInterface));

            wrapperCtor = wrapperType.CreateType()!
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(TValue) }, null)!;
        }

        public static TWithInterface Wrap(TValue value)
        {
            EnsureArg.HasValue(value);

            return (TWithInterface)wrapperCtor.Invoke(new object[] { value })!;
        }

        public static bool TryWrap(TValue value, out TWithInterface? withInterface)
        {
            EnsureArg.HasValue(value);

            if (CompletelyMapped)
            {
                withInterface = (TWithInterface)wrapperCtor.Invoke(new object[] { value })!;
                return true;
            }

            withInterface = default;
            return false;
        }

        private static TypeBuilder CreateType(ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.DefineType(
                $"{typeof(DuckTyper).FullName}._generated.{typeof(TValue).FullName}",
                TypeAttributes.NotPublic |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null);
        }

        private static FieldBuilder CreateValueField(TypeBuilder wrapperTypeBuilder)
        {
            return wrapperTypeBuilder.DefineField(
                "innerValue",
                typeof(TValue),
                FieldAttributes.Private | FieldAttributes.InitOnly);
        }

        private static ConstructorBuilder CreateConstructor(TypeBuilder wrapperTypeBuilder, FieldBuilder innerValueFieldBuilder)
        {
            var constructor = wrapperTypeBuilder.DefineConstructor(
                MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
                CallingConventions.Standard | CallingConventions.HasThis,
                new[] { typeof(TValue) });

            var ilGen = constructor.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
            ilGen.Emit(OpCodes.Nop);
            ilGen.Emit(OpCodes.Nop);
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Stfld, innerValueFieldBuilder);
            ilGen.Emit(OpCodes.Ret);

            return constructor;
        }

        private static bool HandleMember(TypeBuilder wrapperType, FieldBuilder innerValue, MemberInfo interfaceMember)
        {
            var innerValueMember = typeof(TValue)
                .GetMembers()
                .SingleOrDefault(x => x.Name == interfaceMember.Name);

            if (interfaceMember is PropertyInfo interfaceProperty)
            {
                if (innerValueMember is FieldInfo innerValueField)
                {
                    if (interfaceProperty.PropertyType == innerValueField.FieldType)
                    {
                        return HandleField(wrapperType, innerValue, interfaceProperty, innerValueField);
                    }
                    else
                    {
                        return HandleMissingFieldOrProperty(wrapperType, innerValue, interfaceProperty);
                    }
                }
                else if (innerValueMember is PropertyInfo innerValueProperty)
                {
                    if (interfaceProperty.PropertyType == innerValueProperty.PropertyType)
                    {
                        return HandleProperty(wrapperType, innerValue, interfaceProperty, innerValueProperty);
                    }
                    else
                    {
                        return HandleMissingFieldOrProperty(wrapperType, innerValue, interfaceProperty);
                    }
                }
                else
                {
                    return HandleMissingFieldOrProperty(wrapperType, innerValue, interfaceProperty);
                }
            }
            else if (interfaceMember is MethodInfo interfaceMethod)
            {
                if (innerValueMember is MethodInfo innerValueMethod)
                {
                    return HandleMethod(wrapperType, innerValue, interfaceMethod, innerValueMethod);
                }
                else
                {
                    return HandleMissingMethod(wrapperType, interfaceMethod);
                }
            }

            return false;
        }

        private static bool HandleField(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyInfo interfaceProperty, FieldInfo innerValueField)
        {
            var interfaceGetMethodInfo = interfaceProperty.GetGetMethod();
            var interfaceSetMethodInfo = interfaceProperty.GetSetMethod();
            var parameters = (interfaceGetMethodInfo ?? interfaceSetMethodInfo!).GetParameters();

            var wrapperProperty = wrapperType.DefineProperty(
                interfaceProperty.Name,
                PropertyAttributes.None,
                interfaceProperty.PropertyType,
                parameters.Select(x => x.ParameterType).ToArray());

            var mappedCompletely = true;

            if (interfaceGetMethodInfo != null)
            {
                mappedCompletely &= HandleFieldGetter(wrapperType, innerValue, wrapperProperty, innerValueField);
            }

            if (interfaceSetMethodInfo != null)
            {
                if (innerValueField.IsInitOnly)
                {
                    mappedCompletely &= HandleMissingFieldOrPropertySetter(wrapperType, wrapperProperty);
                }
                else
                {
                    mappedCompletely &= HandleFieldSetter(wrapperType, innerValue, wrapperProperty, innerValueField);
                }
            }

            return mappedCompletely;
        }

        private static bool HandleFieldGetter(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyBuilder wrapperProperty, FieldInfo innerValueField)
        {
            var wrapperPropertyGetMethod = wrapperType.DefineMethod(
                $"get_{wrapperProperty.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                wrapperProperty.PropertyType,
                null);

            var ilGen = wrapperPropertyGetMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, innerValue);
            ilGen.Emit(OpCodes.Ldfld, innerValueField);
            ilGen.Emit(OpCodes.Ret);

            wrapperProperty.SetGetMethod(wrapperPropertyGetMethod);

            return true;
        }

        private static bool HandleFieldSetter(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyBuilder wrapperProperty, FieldInfo innerValueField)
        {
            var wrapperPropertySetMethod = wrapperType.DefineMethod(
                $"set_{wrapperProperty.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                null,
                new[] { wrapperProperty.PropertyType });

            var ilGen = wrapperPropertySetMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, innerValue);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Stfld, innerValueField);
            ilGen.Emit(OpCodes.Ret);

            wrapperProperty.SetSetMethod(wrapperPropertySetMethod);

            return true;
        }

        private static bool HandleProperty(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyInfo interfaceProperty, PropertyInfo innerValueProperty)
        {
            var interfaceGetMethodInfo = interfaceProperty.GetGetMethod();
            var interfaceSetMethodInfo = interfaceProperty.GetSetMethod();
            var parameters = (interfaceGetMethodInfo ?? interfaceSetMethodInfo!).GetParameters();

            var wrapperProperty = wrapperType.DefineProperty(
                interfaceProperty.Name,
                PropertyAttributes.None,
                interfaceProperty.PropertyType,
                parameters.Select(x => x.ParameterType).ToArray());

            var innerValuePropertyGetter = innerValueProperty.GetGetMethod();
            var innerValuePropertySetter = innerValueProperty.GetSetMethod();

            var mappedCompletely = true;

            if (interfaceGetMethodInfo != null)
            {
                if (innerValuePropertyGetter != null)
                {
                    mappedCompletely &= HandlePropertyGetter(wrapperType, innerValue, wrapperProperty, innerValuePropertyGetter);
                }
                else
                {
                    mappedCompletely &= HandleMissingFieldOrPropertyGetter(wrapperType, wrapperProperty);
                }
            }

            if (interfaceSetMethodInfo != null)
            {
                if (innerValuePropertySetter != null)
                {
                    mappedCompletely &= HandlePropertySetter(wrapperType, innerValue, wrapperProperty, innerValuePropertySetter);
                }
                else
                {
                    mappedCompletely &= HandleMissingFieldOrPropertySetter(wrapperType, wrapperProperty);
                }
            }

            return mappedCompletely;
        }

        private static bool HandlePropertyGetter(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyBuilder wrapperProperty, MethodInfo innerValuePropertyGetter)
        {
            var valuePropertyGetMethod = wrapperType.DefineMethod(
                $"get_{wrapperProperty.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                wrapperProperty.PropertyType,
                null);

            var callOpCode = typeof(TValue).IsValueType ? OpCodes.Call : OpCodes.Callvirt;

            var ilGen = valuePropertyGetMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, innerValue);
            ilGen.Emit(callOpCode, innerValuePropertyGetter);
            ilGen.Emit(OpCodes.Ret);

            wrapperProperty.SetGetMethod(valuePropertyGetMethod);

            return true;
        }

        private static bool HandlePropertySetter(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyBuilder wrapperProperty, MethodInfo innerValuePropertySetter)
        {
            var valuePropertySetMethod = wrapperType.DefineMethod(
                $"set_{wrapperProperty.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                null,
                new[] { wrapperProperty.PropertyType });

            var callOpCode = typeof(TValue).IsValueType ? OpCodes.Call : OpCodes.Callvirt;

            var ilGen = valuePropertySetMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, innerValue);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(callOpCode, innerValuePropertySetter);
            ilGen.Emit(OpCodes.Nop);
            ilGen.Emit(OpCodes.Ret);

            wrapperProperty.SetSetMethod(valuePropertySetMethod);

            return true;
        }

        private static bool HandleMissingFieldOrProperty(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyInfo interfaceProperty)
        {
            var interfaceGetMethodInfo = interfaceProperty.GetGetMethod();
            var interfaceSetMethodInfo = interfaceProperty.GetSetMethod();
            var parameters = (interfaceGetMethodInfo ?? interfaceSetMethodInfo!).GetParameters();

            var wrapperProperty = wrapperType.DefineProperty(
                interfaceProperty.Name,
                PropertyAttributes.None,
                interfaceProperty.PropertyType,
                parameters.Select(x => x.ParameterType).ToArray());

            if (interfaceGetMethodInfo != null)
                HandleMissingFieldOrPropertyGetter(wrapperType, wrapperProperty);

            if (interfaceSetMethodInfo != null)
                HandleMissingFieldOrPropertySetter(wrapperType, wrapperProperty);

            return false;
        }

        private static bool HandleMissingFieldOrPropertyGetter(TypeBuilder wrapperType, PropertyBuilder wrapperProperty)
        {
            var wrapperPropertyGetMethod = wrapperType.DefineMethod(
                $"get_{wrapperProperty.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                wrapperProperty.PropertyType,
                null);

            var ilGen = wrapperPropertyGetMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldstr, $"The field or property {wrapperProperty.Name} does not exist or does not have a get_{wrapperProperty.Name} method that returns {wrapperProperty.PropertyType.Name} on the wrapped type {typeof(TValue).Name}");
            ilGen.Emit(OpCodes.Newobj, DuckTyper.InvalidOperationExceptionCtor);
            ilGen.Emit(OpCodes.Throw);

            wrapperProperty.SetGetMethod(wrapperPropertyGetMethod);

            return false;
        }

        private static bool HandleMissingFieldOrPropertySetter(TypeBuilder wrapperType, PropertyBuilder wrapperProperty)
        {
            var wrapperPropertySetMethod = wrapperType.DefineMethod(
                $"set_{wrapperProperty.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                null,
                new[] { wrapperProperty.PropertyType });

            var ilGen = wrapperPropertySetMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldstr, $"The field or property {wrapperProperty.Name} does not exist or does not have a set_{wrapperProperty.Name} method that returns {wrapperProperty.PropertyType.Name} on the wrapped type {typeof(TValue).Name}");
            ilGen.Emit(OpCodes.Newobj, DuckTyper.InvalidOperationExceptionCtor);
            ilGen.Emit(OpCodes.Throw);

            wrapperProperty.SetSetMethod(wrapperPropertySetMethod);

            return false;
        }

        private static bool HandleMethod(TypeBuilder wrapperType, FieldBuilder innerValue, MethodInfo interfaceMethod, MethodInfo innerValueMethod)
        {
            var parameters = interfaceMethod.GetParameters();

            var wrapperProperty = wrapperType.DefineMethod(
                interfaceMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                interfaceMethod.ReturnType,
                parameters.Select(x => x.ParameterType).ToArray());

            var ilGen = wrapperProperty.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, innerValue);

            var callOpCode = typeof(TValue).IsValueType ? OpCodes.Call : OpCodes.Callvirt;

            if (parameters.Any())
            {
                if(interfaceMethod.ReturnType == null)
                {
                    ilGen.Emit(OpCodes.Ldarg_1);
                    ilGen.Emit(callOpCode, innerValueMethod);
                    ilGen.Emit(OpCodes.Nop);
                    ilGen.Emit(OpCodes.Ret);
                }
                else
                {
                    ilGen.Emit(OpCodes.Ldarg_1);
                    ilGen.Emit(callOpCode, innerValueMethod);
                    ilGen.Emit(OpCodes.Ret);
                }
            }
            else
            {
                if (interfaceMethod.ReturnType == null)
                {
                    ilGen.Emit(callOpCode, innerValueMethod);
                    ilGen.Emit(OpCodes.Nop);
                    ilGen.Emit(OpCodes.Ret);
                }
                else
                {
                    ilGen.Emit(callOpCode, innerValueMethod);
                    ilGen.Emit(OpCodes.Ret);
                }
            }

            return true;
        }

        private static bool HandleMissingMethod(TypeBuilder wrapperType, MethodInfo interfaceMethod)
        {
            var parameters = interfaceMethod.GetParameters();

            var wrapperMethod = wrapperType.DefineMethod(
                interfaceMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                interfaceMethod.ReturnType,
                parameters.Select(x => x.ParameterType).ToArray());

            var ilGen = wrapperMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldstr, $"The method {wrapperMethod.Name} does not exist or does not return {wrapperMethod.ReturnType.Name} on the wrapped type {typeof(TValue).Name}");
            ilGen.Emit(OpCodes.Newobj, DuckTyper.InvalidOperationExceptionCtor);
            ilGen.Emit(OpCodes.Throw);

            return false;
        }
    }
}
