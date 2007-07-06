using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualMethodInterfaceInterceptor
    {
        static readonly Dictionary<Type, Type> wrappers = new Dictionary<Type, Type>();

        static MethodBuilder GenerateAnonymousDelegate(MethodInfo methodToIntercept,
                                                       TypeBuilder typeBuilder,
                                                       FieldInfo fieldTarget)
        {
            ParameterInfo[] parameters = methodToIntercept.GetParameters();

            MethodBuilder anonymousDelegate =
                typeBuilder.DefineMethod(methodToIntercept.Name + "<Delegate>",
                                         MethodAttributes.Private | MethodAttributes.HideBySig,
                                         typeof(object),
                                         new Type[] { typeof(object[]) });

            ILGenerator il = anonymousDelegate.GetILGenerator();

            il.DeclareLocal(typeof(object));

            foreach (ParameterInfo parameter in parameters)
                if (parameter.IsOut || parameter.ParameterType.IsByRef)
                    il.DeclareLocal(parameter.ParameterType.GetElementType());

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldTarget);

            int localIndex = 1;

            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                if (parameters[idx].ParameterType.IsByRef && !parameters[idx].IsOut)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4_S, idx);
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (parameters[idx].ParameterType.GetElementType().IsValueType)
                        il.Emit(OpCodes.Unbox_Any, parameters[idx].ParameterType.GetElementType());
                    else
                        il.Emit(OpCodes.Castclass, parameters[idx].ParameterType.GetElementType());

                    il.Emit(OpCodes.Stloc_S, localIndex++);
                }
            }

            localIndex = 1;

            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldloca_S, localIndex++);
                else
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4_S, idx);
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (parameters[idx].ParameterType.IsValueType)
                        il.Emit(OpCodes.Unbox_Any, parameters[idx].ParameterType);
                    else
                        il.Emit(OpCodes.Castclass, parameters[idx].ParameterType);
                }
            }

            il.Emit(OpCodes.Callvirt, methodToIntercept);

            if (methodToIntercept.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ldnull);
            }
            else if (methodToIntercept.ReturnType.IsValueType)
                il.Emit(OpCodes.Box, methodToIntercept.ReturnType);

            il.Emit(OpCodes.Stloc_0);

            localIndex = 1;

            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4_S, idx);
                    il.Emit(OpCodes.Ldloc_S, localIndex++);

                    if (parameters[idx].ParameterType.GetElementType().IsValueType)
                        il.Emit(OpCodes.Box, parameters[idx].ParameterType.GetElementType());

                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            Label end = il.DefineLabel();
            il.Emit(OpCodes.Br_S, end);
            il.MarkLabel(end);

            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            return anonymousDelegate;
        }

        static void GenerateConstructor(TypeBuilder typeBuilder,
                                        Type interfaceToWrap,
                                        FieldInfo fieldProxy,
                                        FieldInfo fieldTarget,
                                        IEnumerable<KeyValuePair<FieldBuilder, MethodInfo>> overloadedMethods)
        {
            ConstructorBuilder constructor =
                typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                                              CallingConventions.HasThis,
                                              new Type[] { typeof(VirtualMethodProxy), interfaceToWrap });

            ConstructorInfo defaultBaseConstructor = typeof(Object).GetConstructor(new Type[0]);

            ILGenerator il = constructor.GetILGenerator();

            // Locals
            il.DeclareLocal(typeof(Type[]));

            // Call base constructor
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, defaultBaseConstructor);
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Nop);

            // Stash proxy into field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldProxy);

            // Stash target into field
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, fieldTarget);

            MethodInfo getTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
            MethodInfo getMethodMethod = typeof(Type).GetMethod("GetMethod", new Type[] { typeof(string), typeof(Type[]) });
            MethodInfo makeByRefTypeMethod = typeof(Type).GetMethod("MakeByRefType");

            // Get interface methods via reflection
            foreach (KeyValuePair<FieldBuilder, MethodInfo> kvp in overloadedMethods)
            {
                ParameterInfo[] parameters = kvp.Value.GetParameters();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldtoken, interfaceToWrap);
                il.Emit(OpCodes.Call, getTypeFromHandleMethod);
                il.Emit(OpCodes.Ldstr, kvp.Value.Name);
                il.Emit(OpCodes.Ldc_I4_S, parameters.Length);
                il.Emit(OpCodes.Newarr, typeof(Type));
                il.Emit(OpCodes.Stloc_0);

                for (int idx = 0; idx < parameters.Length; ++idx)
                {
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldc_I4_S, idx);

                    if (parameters[idx].ParameterType.IsByRef)
                    {
                        il.Emit(OpCodes.Ldtoken, parameters[idx].ParameterType.GetElementType());
                        il.Emit(OpCodes.Call, getTypeFromHandleMethod);
                        il.Emit(OpCodes.Callvirt, makeByRefTypeMethod);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldtoken, parameters[idx].ParameterType);
                        il.Emit(OpCodes.Call, getTypeFromHandleMethod);
                    }

                    il.Emit(OpCodes.Stelem_Ref);
                }

                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Call, getMethodMethod);
                il.Emit(OpCodes.Stfld, kvp.Key);
            }

            // Return
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Ret);
        }

        static void GenerateOverloadedMethod(TypeBuilder typeBuilder,
                                             MethodInfo methodToIntercept,
                                             MethodInfo anonymousDelegate,
                                             FieldInfo fieldProxy,
                                             FieldInfo fieldTarget,
                                             FieldInfo fieldMethodInfo)
        {
            ParameterInfo[] parameters = methodToIntercept.GetParameters();
            List<Type> parameterTypes = new List<Type>();
            List<Type> parameterRealTypes = new List<Type>();

            foreach (ParameterInfo parameter in parameters)
            {
                parameterTypes.Add(parameter.ParameterType);

                if (parameter.IsOut || parameter.ParameterType.IsByRef)
                    parameterRealTypes.Add(parameter.ParameterType.GetElementType());
                else
                    parameterRealTypes.Add(parameter.ParameterType);
            }

            // Create overriden method
            MethodBuilder method =
                typeBuilder.DefineMethod(methodToIntercept.Name,
                                         MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final,
                                         methodToIntercept.ReturnType,
                                         parameterTypes.ToArray());

            ILGenerator il = method.GetILGenerator();

            // Locals
            il.DeclareLocal(typeof(object[]));

            if (methodToIntercept.ReturnType != typeof(void))
                il.DeclareLocal(methodToIntercept.ReturnType);

            il.Emit(OpCodes.Nop);

            // Initialize default values for out parameters
            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                if (parameters[idx].IsOut && !parameters[idx].IsIn)
                {
                    il.Emit(OpCodes.Ldarg_S, idx + 1);
                    il.Emit(OpCodes.Initobj, parameterRealTypes[idx]);
                }
            }

            // Create an array equal to the size of the # of parameters
            il.Emit(OpCodes.Ldc_I4_S, parameters.Length);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc_0);

            // Populate the array
            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldc_I4_S, idx);
                il.Emit(OpCodes.Ldarg_S, idx + 1);

                if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldobj, parameterRealTypes[idx]);

                if (parameterRealTypes[idx].IsValueType)
                    il.Emit(OpCodes.Box, parameterRealTypes[idx]);

                il.Emit(OpCodes.Stelem_Ref);
            }

            // Parameter 0 (this argument) for the call to Invoke
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldProxy);

            // Parameter 1 (target) for the call to Invoke
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldTarget);

            // Parameter 2 (method) for the call to Invoke
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldMethodInfo);

            // Parameter 3 (parameter array) for the call to Invoke
            il.Emit(OpCodes.Ldloc_0);

            // Parameter 4 (anonymous delegate) for the call to Invoke
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldftn, anonymousDelegate);
            il.Emit(OpCodes.Newobj, typeof(VirtualMethodProxy.InvokeDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));

            // Call Invoke
            il.Emit(OpCodes.Callvirt, typeof(VirtualMethodProxy).GetMethod("Invoke"));

            // Retrieve the return value from Invoke
            if (methodToIntercept.ReturnType == typeof(void))
                il.Emit(OpCodes.Pop);
            else
            {
                // Cast or unbox, dependening on whether it's a class or value type
                if (methodToIntercept.ReturnType.IsClass)
                    il.Emit(OpCodes.Castclass, methodToIntercept.ReturnType);
                else
                    il.Emit(OpCodes.Unbox_Any, methodToIntercept.ReturnType);

                // Store the value into the temporary
                il.Emit(OpCodes.Stloc_1);

                // (Seemingly unnecessary?) branch
                Label end = il.DefineLabel();
                il.Emit(OpCodes.Br_S, end);
                il.MarkLabel(end);

                // Load the return value
                il.Emit(OpCodes.Ldloc_1);
            }

            // Set out/ref values before returning
            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_S, idx + 1);
                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldc_I4_S, idx);
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (parameterRealTypes[idx].IsValueType)
                        il.Emit(OpCodes.Unbox_Any, parameterRealTypes[idx]);
                    else
                        il.Emit(OpCodes.Castclass, parameterRealTypes[idx]);

                    il.Emit(OpCodes.Stobj, parameterRealTypes[idx]);
                }
            }

            il.Emit(OpCodes.Ret);
        }

        static Type GenerateWrapperType(Type interfaceToWrap,
                                        ModuleBuilder module)
        {
            TypeBuilder typeBuilder = module.DefineType(
                interfaceToWrap.FullName + "<Wrapper>",
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
                typeof(Object),
                new Type[] { interfaceToWrap });

            Dictionary<FieldBuilder, MethodInfo> overloadedMethods = new Dictionary<FieldBuilder, MethodInfo>();

            FieldBuilder fieldProxy = typeBuilder.DefineField("proxy",
                                                              typeof(VirtualMethodProxy),
                                                              FieldAttributes.Private);
            FieldBuilder fieldTarget = typeBuilder.DefineField("target",
                                                               interfaceToWrap,
                                                               FieldAttributes.Private);

            foreach (MethodInfo method in interfaceToWrap.GetMethods())
                if (method.IsVirtual && !method.IsFinal)
                {
                    FieldBuilder field = typeBuilder.DefineField("methodInfo" + overloadedMethods.Count,
                                                                 typeof(MethodInfo),
                                                                 FieldAttributes.Private);

                    MethodBuilder anonymousDelegate = GenerateAnonymousDelegate(method, typeBuilder, fieldTarget);
                    GenerateOverloadedMethod(typeBuilder, method, anonymousDelegate, fieldProxy, fieldTarget, field);

                    overloadedMethods[field] = method;
                }

            GenerateConstructor(typeBuilder, interfaceToWrap, fieldProxy, fieldTarget, overloadedMethods);
            return typeBuilder.CreateType();
        }

        public static Type WrapInterface(Type interfaceToWrap)
        {
            lock (wrappers)
            {
                if (!wrappers.ContainsKey(interfaceToWrap))
                {
                    string assemblyName = Guid.NewGuid().ToString("N");
                    AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
                    ModuleBuilder module = assemblyBuilder.DefineDynamicModule(assemblyName + ".dll");
                    wrappers[interfaceToWrap] = GenerateWrapperType(interfaceToWrap, module);
                }

                return wrappers[interfaceToWrap];
            }
        }
    }
}