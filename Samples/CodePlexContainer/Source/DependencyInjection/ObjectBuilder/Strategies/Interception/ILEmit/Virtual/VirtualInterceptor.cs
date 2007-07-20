using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualInterceptor
    {
        static readonly Dictionary<Type, Type> wrappers = new Dictionary<Type, Type>();

        static MethodBuilder GenerateAnonymousDelegate(MethodInfo methodToIntercept,
                                                       TypeBuilder typeBuilder)
        {
            ParameterInfo[] parameters = methodToIntercept.GetParameters();
            List<string> genericParameterNames = new List<string>();

            foreach (Type type in methodToIntercept.GetGenericArguments())
                genericParameterNames.Add(type.Name);

            MethodBuilder anonymousDelegate =
                typeBuilder.DefineMethod(methodToIntercept.Name + "{Delegate}",
                                         MethodAttributes.Private | MethodAttributes.HideBySig,
                                         typeof(object),
                                         new Type[] { typeof(object[]) });

            if (genericParameterNames.Count > 0)
                anonymousDelegate.DefineGenericParameters(genericParameterNames.ToArray());

            ILGenerator il = anonymousDelegate.GetILGenerator();

            il.DeclareLocal(typeof(object));

            foreach (ParameterInfo parameter in parameters)
                if (parameter.IsOut || parameter.ParameterType.IsByRef)
                    il.DeclareLocal(parameter.ParameterType.GetElementType());

            il.Emit(OpCodes.Nop);

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

            il.Emit(OpCodes.Ldarg_0);

            localIndex = 1;

            for (int idx = 0; idx < parameters.Length; ++idx)
            {
                if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldloca_S, localIndex++);
                }
                else
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4_S, idx);
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (parameters[idx].ParameterType.IsValueType || parameters[idx].ParameterType.IsGenericParameter)
                        il.Emit(OpCodes.Unbox_Any, parameters[idx].ParameterType);
                }
            }

            il.Emit(OpCodes.Call, methodToIntercept.GetBaseDefinition());

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

        static void GenerateConstructors(TypeBuilder typeBuilder,
                                         Type classToWrap,
                                         FieldInfo fieldProxy)
        {
            foreach (ConstructorInfo constructor in classToWrap.GetConstructors())
            {
                List<Type> parameterTypes = new List<Type>();

                parameterTypes.Add(typeof(ILEmitProxy));
                foreach (ParameterInfo parameterInfo in constructor.GetParameters())
                    parameterTypes.Add(parameterInfo.ParameterType);

                ConstructorBuilder wrappedConstructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, parameterTypes.ToArray());
                ILGenerator il = wrappedConstructor.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);

                for (int i = 0; i < constructor.GetParameters().Length; i++)
                    il.Emit(OpCodes.Ldarg_S, i + 2);

                il.Emit(OpCodes.Call, constructor);
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, fieldProxy);
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ret);
            }
        }

        static void GenerateOverloadedMethod(TypeBuilder typeBuilder,
                                             MethodInfo methodToIntercept,
                                             MethodInfo anonymousDelegate,
                                             FieldInfo fieldProxy)
        {
            // Get parameters
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

            // Get generic parameters
            List<Type> genericParameterTypes = new List<Type>(methodToIntercept.GetGenericArguments());
            List<string> genericParameterNames = new List<string>();

            foreach (Type type in genericParameterTypes)
                genericParameterNames.Add(type.Name);

            // Create overriden method
            MethodBuilder method =
                typeBuilder.DefineMethod(methodToIntercept.Name,
                                         MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                         methodToIntercept.ReturnType,
                                         parameterTypes.ToArray());

            if (genericParameterNames.Count > 0)
                method.DefineGenericParameters(genericParameterNames.ToArray());

            ILGenerator il = method.GetILGenerator();

            // Locals
            il.DeclareLocal(typeof(MethodInfo));
            il.DeclareLocal(typeof(object[]));

            if (methodToIntercept.ReturnType != typeof(void))
                il.DeclareLocal(methodToIntercept.ReturnType);

            // NOP to start us off
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
                il.Emit(OpCodes.Ldarg_S, idx + 1);

                if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldobj, parameterRealTypes[idx]);

                if (parameterRealTypes[idx].IsValueType || parameterRealTypes[idx].IsGenericParameter)
                    il.Emit(OpCodes.Box, parameterRealTypes[idx]);

                il.Emit(OpCodes.Stelem_Ref);
            }

            // Parameter 0 (this argument) for the call to Invoke
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldProxy);

            // Parameter 1 (target) for the call to Invoke
            il.Emit(OpCodes.Ldarg_0);

            // Parameter 2 (method) for the call to Invoke
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Callvirt, typeof(MethodInfo).GetMethod("GetBaseDefinition"));

            // Parameter 3 (parameter array) for the call to Invoke
            il.Emit(OpCodes.Ldloc_1);

            // Parameter 4 (anonymous delegate) for the call to Invoke
            il.Emit(OpCodes.Ldarg_0);

            if (genericParameterTypes.Count > 0)
                il.Emit(OpCodes.Ldftn, anonymousDelegate.MakeGenericMethod(genericParameterTypes.ToArray()));
            else
                il.Emit(OpCodes.Ldftn, anonymousDelegate);

            il.Emit(OpCodes.Newobj, typeof(ILEmitProxy.InvokeDelegate).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));

            // Call Invoke
            il.Emit(OpCodes.Callvirt, typeof(ILEmitProxy).GetMethod("Invoke"));

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
                il.Emit(OpCodes.Stloc_2);

                // (Seemingly unnecessary?) branch
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

                    if (parameterRealTypes[idx].IsValueType)
                        il.Emit(OpCodes.Unbox_Any, parameterRealTypes[idx]);
                    else
                        il.Emit(OpCodes.Castclass, parameterRealTypes[idx]);

                    il.Emit(OpCodes.Stobj, parameterRealTypes[idx]);
                }
            }

            il.Emit(OpCodes.Ret);
        }

        static Type GenerateWrapperType(Type classToWrap,
                                        ModuleBuilder module)
        {
            TypeBuilder typeBuilder = module.DefineType(
                classToWrap.FullName + "{Wrapper}",
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.BeforeFieldInit,
                classToWrap);

            if (classToWrap.IsGenericTypeDefinition)
            {
                Type[] arguments = classToWrap.GetGenericArguments();
                string[] parameters = new string[arguments.Length];
                for (int idx = 0; idx < arguments.Length; idx++)
                {
                    parameters[idx] = arguments[idx].Name;
                }
                typeBuilder.DefineGenericParameters(parameters);
            }

            FieldBuilder fieldProxy = typeBuilder.DefineField("proxy", typeof(ILEmitProxy), FieldAttributes.Private);

            foreach (MethodInfo method in classToWrap.GetMethods())
                if (method.IsVirtual && !method.IsFinal)
                {
                    MethodInfo anonymousDelegate = GenerateAnonymousDelegate(method, typeBuilder);
                    GenerateOverloadedMethod(typeBuilder, method, anonymousDelegate, fieldProxy);
                }

            GenerateConstructors(typeBuilder, classToWrap, fieldProxy);
            return typeBuilder.CreateType();
        }

        public static Type WrapClass(Type classToWrap)
        {
            lock (wrappers)
            {
                Type actualClassToWrap = classToWrap;

                if (classToWrap.IsGenericType)
                    actualClassToWrap = classToWrap.GetGenericTypeDefinition();

                if (!wrappers.ContainsKey(actualClassToWrap))
                {
                    string assemblyName = Guid.NewGuid().ToString("N");
                    AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
                    ModuleBuilder module = assemblyBuilder.DefineDynamicModule(assemblyName + ".dll", "GeneratedStuff.dll", true);
                    wrappers[actualClassToWrap] = GenerateWrapperType(actualClassToWrap, module);
                    assemblyBuilder.Save("Foo.dll");
                }

                if (actualClassToWrap != classToWrap)
                    return wrappers[actualClassToWrap].MakeGenericType(classToWrap.GetGenericArguments());

                return wrappers[classToWrap];
            }
        }
    }
}