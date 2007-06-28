using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualMethodInterceptor<T>
    {
        static void GenerateConstructor(TypeBuilder typeBuilder,
                                        Type targetType,
                                        FieldInfo fieldProxy,
                                        FieldInfo fieldTarget)
        {
            ConstructorBuilder constructor =
                typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                                              CallingConventions.HasThis,
                                              new Type[] { typeof(VirtualMethodProxy), typeof(object) });

            ConstructorInfo defaultBaseConstructor = targetType.BaseType.GetConstructor(new Type[0]);

            if (defaultBaseConstructor == null)
                throw new InvalidOperationException("Could not find a suitable default constructor on " + targetType.FullName);

            ILGenerator il = constructor.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, defaultBaseConstructor);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldProxy);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, fieldTarget);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);
        }

        static object GenerateObject(T target,
                                     IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers)
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "InterceptedClasses";
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder module = assemblyBuilder.DefineDynamicModule("InterceptedClasses.dll");
            Type wrapperType = GenerateWrapperType(typeof(T), module);
            VirtualMethodProxy proxy = new VirtualMethodProxy(handlers);
            ConstructorInfo ci = wrapperType.GetConstructor(new Type[] { typeof(VirtualMethodProxy), typeof(object) });
            return ci.Invoke(new object[] { proxy, target });
        }

        static void GenerateOverloadedMethod(TypeBuilder typeBuilder,
                                             MethodInfo methodInfo,
                                             FieldInfo fieldProxy,
                                             FieldInfo fieldTarget)
        {
            string methodName = methodInfo.Name;
            ParameterInfo[] parameters = methodInfo.GetParameters();
            List<Type> parameterTypes = new List<Type>();

            foreach (ParameterInfo parameter in parameters)
                parameterTypes.Add(parameter.ParameterType);

            MethodBuilder method =
                typeBuilder.DefineMethod(methodName,
                                         MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual, methodInfo.ReturnType,
                                         parameterTypes.ToArray());

            ILGenerator il = method.GetILGenerator();

            // Locals
            il.DeclareLocal(typeof(MethodInfo));
            il.DeclareLocal(typeof(object[]));

            if (methodInfo.ReturnType != typeof(void))
                il.DeclareLocal(methodInfo.ReturnType);

            // NOP to start us off
            il.Emit(OpCodes.Nop);

            // Initialize default values for out parameters
            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                if (parameters[idx].IsOut && !parameters[idx].IsIn)
                {
                    il.Emit(OpCodes.Ldarg_S, idx + 1);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Stind_Ref);
                }
            }

            // Call to MethodInfo.GetCurrentMethod() and cast to MethodInfo
            il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetCurrentMethod", BindingFlags.Static | BindingFlags.Public));
            il.Emit(OpCodes.Castclass, typeof(MethodInfo));
            il.Emit(OpCodes.Stloc_0);

            // Create an array equal to the size of the # of parameters
            il.Emit(OpCodes.Ldc_I4_S, parameters.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc_1);

            // Populate the array
            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Ldc_I4_S, idx);
                il.Emit(OpCodes.Ldarg_S, idx + 1); // +1 for the "this" parameter

                if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
                    if (parameters[idx].ParameterType.GetElementType().IsValueType)
                        il.Emit(OpCodes.Ldind_I4);
                    else
                        il.Emit(OpCodes.Ldind_Ref);

                if (parameters[idx].ParameterType.IsValueType)
                    il.Emit(OpCodes.Box, parameters[idx].ParameterType);

                il.Emit(OpCodes.Stelem_Ref);
            }

            // Parameter 0 (this) for the call to Invoke
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldProxy);

            // Parameter 1 (target) for the call to Invoke
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldTarget);

            // Parameter 2 (method) for the call to Invoke
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Callvirt, typeof(MethodInfo).GetMethod("GetBaseDefinition"));

            // Parameter 3 (parameter array) for the call to Invoke
            il.Emit(OpCodes.Ldloc_1);

            // Call Invoke
            il.Emit(OpCodes.Callvirt, typeof(VirtualMethodProxy).GetMethod("Invoke"));

            // Retrieve the return value from Invoke
            if (methodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Pop);
            else
            {
                // Cast or unbox, dependening on whether it's a class or value type
                if (methodInfo.ReturnType.IsClass)
                    il.Emit(OpCodes.Castclass, methodInfo.ReturnType);
                else
                    il.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);

                // Store the value into the temporary
                il.Emit(OpCodes.Stloc_2);

                // Seemingly unnecessary branch
                Label end = il.DefineLabel();
                il.Emit(OpCodes.Br_S, end);
                il.MarkLabel(end);

                // Load the return value
                il.Emit(OpCodes.Ldloc_2);
            }

            // Set out/ref values before returning
            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_S, idx + 1);
                    il.Emit(OpCodes.Ldloc_1);
                    il.Emit(OpCodes.Ldc_I4_S, idx);
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (parameters[idx].ParameterType.GetElementType().IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, parameters[idx].ParameterType.GetElementType());
                        il.Emit(OpCodes.Stind_I4);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, parameters[idx].ParameterType.GetElementType());
                        il.Emit(OpCodes.Stind_Ref);
                    }
                }
            }

            il.Emit(OpCodes.Ret);
        }

        static Type GenerateWrapperType(Type targetType,
                                        ModuleBuilder module)
        {
            TypeBuilder typeBuilder = module.DefineType(
                targetType.Name + "__Wrapper",
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
                targetType);

            FieldBuilder fieldProxy = typeBuilder.DefineField("proxy", typeof(VirtualMethodProxy), FieldAttributes.Private);
            FieldBuilder fieldTarget = typeBuilder.DefineField("target", typeof(object), FieldAttributes.Private);

            foreach (MethodInfo methodInfo in targetType.GetMethods())
                if (methodInfo.IsPublic && methodInfo.IsVirtual && !methodInfo.IsFinal)
                    GenerateOverloadedMethod(typeBuilder, methodInfo, fieldProxy, fieldTarget);

            GenerateConstructor(typeBuilder, targetType, fieldProxy, fieldTarget);
            return typeBuilder.CreateType();
        }

        public static T Wrap(T target,
                             Dictionary<MethodBase, List<IInterceptionHandler>> handlers)
        {
            return (T)GenerateObject(target, handlers);
        }
    }
}