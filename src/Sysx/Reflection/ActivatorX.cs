using EnsureThat;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Sysx.Enums;

namespace Sysx.Reflection
{
    public class ActivatorX
    {
        internal static readonly ModuleBuilder DynamicModule;
        internal static readonly ConstructorInfo InvalidOperationExceptionCtor;

        static ActivatorX()
        {
            var assemblyName = new AssemblyName($"{nameof(ActivatorX)}_{Guid.NewGuid()}");

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

        public static TAsInterface CreateInstanceAsInterface<TValue, TAsInterface>(TValue value) =>
            ActivatorX<TValue, TAsInterface>.CreateInstanceAsInterface(value);
    }

    public static class ActivatorX<TValue>
    {
        public static TAsInterface CreateInstanceAsInterface<TAsInterface>(TValue value) =>
            ActivatorX<TValue, TAsInterface>.CreateInstanceAsInterface(value);
    }

    public static class ActivatorX<TValue, TAsInterface>
    {
        private static readonly ConstructorInfo wrapperCtor;

        static ActivatorX()
        {
            var wrapperType = CreateType(ActivatorX.DynamicModule);

            var innerValueField = CreateValueField(wrapperType);

            CreateConstructor(wrapperType, innerValueField);

            var handledTypes = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;
            var interfaceMembers = typeof(TAsInterface)
                .GetMembers()
                .Where(x => FlagsEnum.HasAny(x.MemberType, handledTypes))
                .Where(x => x is not MethodInfo xMethod || !xMethod.IsSpecialName)
                .ToArray();

            foreach (var interfaceMember in interfaceMembers)
                HandleMember(wrapperType, innerValueField, interfaceMember);

            wrapperType.AddInterfaceImplementation(typeof(TAsInterface));

            wrapperCtor = wrapperType.CreateType()!
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { typeof(TValue) }, null)!;
        }

        public static TAsInterface CreateInstanceAsInterface(TValue value)
        {
            EnsureArg.HasValue(value);

            return (TAsInterface)wrapperCtor.Invoke(new object[] { value })!;
        }

        private static TypeBuilder CreateType(ModuleBuilder moduleBuilder)
        {
            return moduleBuilder.DefineType(
                $"{typeof(ActivatorX).FullName}._generated.{typeof(TValue).FullName}",
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

        private static void HandleMember(TypeBuilder wrapperType, FieldBuilder innerValue, MemberInfo interfaceMember)
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
                        HandleField(wrapperType, innerValue, interfaceProperty, innerValueField);
                    }
                    else
                    {
                        HandleMissingFieldOrProperty(wrapperType, innerValue, interfaceProperty);
                    }
                }
                else if (innerValueMember is PropertyInfo innerValueProperty)
                {
                    if (interfaceProperty.PropertyType == innerValueProperty.PropertyType)
                    {
                        HandleProperty(wrapperType, innerValue, interfaceProperty, innerValueProperty);
                    }
                    else
                    {
                        HandleMissingFieldOrProperty(wrapperType, innerValue, interfaceProperty);
                    }
                }
                else
                {
                    HandleMissingFieldOrProperty(wrapperType, innerValue, interfaceProperty);
                }
            }
            else if (interfaceMember is MethodInfo interfaceMethod)
            {
                if (innerValueMember is MethodInfo innerValueMethod)
                {
                    HandleMethod(wrapperType, innerValue, interfaceMethod, innerValueMethod);
                }
                else
                {
                    HandleMissingMethod(wrapperType, interfaceMethod);
                }
            }
        }

        private static void HandleField(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyInfo interfaceProperty, FieldInfo innerValueField)
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
            {
                HandleFieldGetter(wrapperType, innerValue, wrapperProperty, innerValueField);
            }

            if (interfaceSetMethodInfo != null)
            {
                if (innerValueField.IsInitOnly)
                {
                    HandleMissingFieldOrPropertySetter(wrapperType, wrapperProperty);
                }
                else
                {
                    HandleFieldSetter(wrapperType, innerValue, wrapperProperty, innerValueField);
                }
            }
        }

        private static void HandleFieldGetter(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyBuilder wrapperProperty, FieldInfo innerValueField)
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
        }

        private static void HandleFieldSetter(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyBuilder wrapperProperty, FieldInfo innerValueField)
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
        }

        private static void HandleProperty(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyInfo interfaceProperty, PropertyInfo innerValueProperty)
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

            if (interfaceGetMethodInfo != null)
            {
                if (innerValuePropertyGetter != null)
                {
                    HandlePropertyGetter(wrapperType, innerValue, wrapperProperty, innerValuePropertyGetter);
                }
                else
                {
                    HandleMissingFieldOrPropertyGetter(wrapperType, wrapperProperty);
                }
            }

            if (interfaceSetMethodInfo != null)
            {
                if (innerValuePropertySetter != null)
                {
                    HandlePropertySetter(wrapperType, innerValue, wrapperProperty, innerValuePropertySetter);
                }
                else
                {
                    HandleMissingFieldOrPropertySetter(wrapperType, wrapperProperty);
                }
            }

        }

        private static void HandlePropertyGetter(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyBuilder wrapperProperty, MethodInfo innerValuePropertyGetter)
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
        }

        private static void HandlePropertySetter(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyBuilder wrapperProperty, MethodInfo innerValuePropertySetter)
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
        }

        private static void HandleMissingFieldOrProperty(TypeBuilder wrapperType, FieldBuilder innerValue, PropertyInfo interfaceProperty)
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
        }

        private static void HandleMissingFieldOrPropertyGetter(TypeBuilder wrapperType, PropertyBuilder wrapperProperty)
        {
            var wrapperPropertyGetMethod = wrapperType.DefineMethod(
                $"get_{wrapperProperty.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                wrapperProperty.PropertyType,
                null);

            var ilGen = wrapperPropertyGetMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldstr, $"The field or property {wrapperProperty.Name} does not exist or does not have a get_{wrapperProperty.Name} method that returns {wrapperProperty.PropertyType.Name} on the wrapped type {typeof(TValue).Name}");
            ilGen.Emit(OpCodes.Newobj, ActivatorX.InvalidOperationExceptionCtor);
            ilGen.Emit(OpCodes.Throw);

            wrapperProperty.SetGetMethod(wrapperPropertyGetMethod);
        }

        private static void HandleMissingFieldOrPropertySetter(TypeBuilder wrapperType, PropertyBuilder wrapperProperty)
        {
            var wrapperPropertySetMethod = wrapperType.DefineMethod(
                $"set_{wrapperProperty.Name}",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                null,
                new[] { wrapperProperty.PropertyType });

            var ilGen = wrapperPropertySetMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldstr, $"The field or property {wrapperProperty.Name} does not exist or does not have a set_{wrapperProperty.Name} method that returns {wrapperProperty.PropertyType.Name} on the wrapped type {typeof(TValue).Name}");
            ilGen.Emit(OpCodes.Newobj, ActivatorX.InvalidOperationExceptionCtor);
            ilGen.Emit(OpCodes.Throw);

            wrapperProperty.SetSetMethod(wrapperPropertySetMethod);
        }

        private static void HandleMethod(TypeBuilder wrapperType, FieldBuilder innerValue, MethodInfo interfaceMethod, MethodInfo innerValueMethod)
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
        }

        private static void HandleMissingMethod(TypeBuilder wrapperType, MethodInfo interfaceMethod)
        {
            var parameters = interfaceMethod.GetParameters();

            var wrapperMethod = wrapperType.DefineMethod(
                interfaceMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                interfaceMethod.ReturnType,
                parameters.Select(x => x.ParameterType).ToArray());

            var ilGen = wrapperMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldstr, $"The method {wrapperMethod.Name} does not exist or does not return {wrapperMethod.ReturnType.Name} on the wrapped type {typeof(TValue).Name}");
            ilGen.Emit(OpCodes.Newobj, ActivatorX.InvalidOperationExceptionCtor);
            ilGen.Emit(OpCodes.Throw);
        }
    }
}
