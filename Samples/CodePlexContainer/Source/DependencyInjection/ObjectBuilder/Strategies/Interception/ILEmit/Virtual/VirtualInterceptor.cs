using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualInterceptor
    {
        static readonly Dictionary<Type, Type> wrappers = new Dictionary<Type, Type>();

        static void GenerateConstructor(TypeBuilder typeBuilder,
                                        ConstructorInfo constructor,
                                        FieldInfo fieldProxy)
        {
            // Get constructor parameters
            List<Type> parameterTypes = new List<Type>();

            parameterTypes.Add(typeof(ILEmitProxy));
            foreach (ParameterInfo parameterInfo in constructor.GetParameters())
                parameterTypes.Add(parameterInfo.ParameterType);

            // Define constructor
            ConstructorBuilder wrappedConstructor = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.HasThis,
                parameterTypes.ToArray());

            ILGenerator il = wrappedConstructor.GetILGenerator();

            // Call base constructor
            il.Emit(OpCodes.Ldarg_0);

            for (int i = 0; i < constructor.GetParameters().Length; i++)
                il.Emit(OpCodes.Ldarg_S, i + 2);

            il.Emit(OpCodes.Call, constructor);

            // Store proxy so it can be used in the overriden methods
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldProxy);

            // Return
            il.Emit(OpCodes.Ret);
        }

        static void GenerateOverloadedMethod(TypeBuilder typeBuilder,
                                             MethodInfo methodToIntercept,
                                             MethodInfo anonymousDelegate,
                                             FieldInfo fieldProxy)
        {
            // Get method parameters
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

            // Define overriden method
            MethodBuilder method =
                typeBuilder.DefineMethod(methodToIntercept.Name,
                                         MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                         methodToIntercept.ReturnType,
                                         parameterTypes.ToArray());

            Type[] genericParameterTypes = SetupGenericMethodArguments(methodToIntercept, method);

            ILGenerator il = method.GetILGenerator();

            // Locals for reflection method info and parameter array, for calls to proxy.Invoke()
            il.DeclareLocal(typeof(MethodInfo));
            il.DeclareLocal(typeof(object[]));

            // Local for the return value
            if (methodToIntercept.ReturnType != typeof(void))
                il.DeclareLocal(methodToIntercept.ReturnType);

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

            if (genericParameterTypes.Length > 0)
                il.Emit(OpCodes.Ldftn, anonymousDelegate.MakeGenericMethod(genericParameterTypes));
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

        static MethodBuilder GenerateOverloadedMethodDelegate(MethodInfo methodToIntercept,
                                                              TypeBuilder typeBuilder)
        {
            // Define the method
            MethodBuilder method =
                typeBuilder.DefineMethod(methodToIntercept.Name + "{Delegate}",
                                         MethodAttributes.Private | MethodAttributes.HideBySig,
                                         typeof(object),
                                         new Type[] { typeof(object[]) });

            SetupGenericMethodArguments(methodToIntercept, method);

            ILGenerator il = method.GetILGenerator();

            // Local for return value
            il.DeclareLocal(typeof(object));

            // Local for each out/ref parameter
            ParameterInfo[] parameters = methodToIntercept.GetParameters();

            foreach (ParameterInfo parameter in parameters)
                if (parameter.IsOut || parameter.ParameterType.IsByRef)
                    il.DeclareLocal(parameter.ParameterType.GetElementType());

            // Initialize out parameters to default values
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

            // Push call values onto stack
            localIndex = 1;

            for (int idx = 0; idx < parameters.Length; ++idx)
                if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldloca_S, localIndex++);
                else
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4_S, idx);
                    il.Emit(OpCodes.Ldelem_Ref);

                    if (parameters[idx].ParameterType.IsValueType || parameters[idx].ParameterType.IsGenericParameter)
                        il.Emit(OpCodes.Unbox_Any, parameters[idx].ParameterType);
                }

            // Call base method
            il.Emit(OpCodes.Call, methodToIntercept.GetBaseDefinition());

            // Stash return value
            if (methodToIntercept.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ldnull);
            }
            else if (methodToIntercept.ReturnType.IsValueType)
                il.Emit(OpCodes.Box, methodToIntercept.ReturnType);

            il.Emit(OpCodes.Stloc_0);

            // Copy out/ref parameter values back into passed-in parameters array
            localIndex = 1;

            for (int idx = 0; idx < parameters.Length; ++idx)
                if (parameters[idx].IsOut || parameters[idx].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldc_I4_S, idx);
                    il.Emit(OpCodes.Ldloc_S, localIndex++);

                    if (parameters[idx].ParameterType.GetElementType().IsValueType)
                        il.Emit(OpCodes.Box, parameters[idx].ParameterType.GetElementType());

                    il.Emit(OpCodes.Stelem_Ref);
                }

            // Return
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            return method;
        }

        static Type GenerateWrapperType(Type classToWrap,
                                        ModuleBuilder module)
        {
            // Define overriding type
            TypeBuilder typeBuilder = module.DefineType("{Wrappers}." + classToWrap.Name, TypeAttributes.Public);
            Type parentType = SetupGenericClassArguments(classToWrap, typeBuilder);

            // Declare a field for the proxy
            FieldBuilder fieldProxy = typeBuilder.DefineField("proxy", typeof(ILEmitProxy), FieldAttributes.Private);

            // Create overrides (and delegates) for all virtual methods
            foreach (MethodInfo method in classToWrap.GetMethods())
                if (method.IsVirtual && !method.IsFinal)
                    GenerateOverloadedMethod(typeBuilder,
                                             method,
                                             GenerateOverloadedMethodDelegate(method, typeBuilder),
                                             fieldProxy);

            // Generate overrides for all constructors
            foreach (ConstructorInfo constructor in classToWrap.GetConstructors())
                GenerateConstructor(typeBuilder, constructor, fieldProxy);

            typeBuilder.SetParent(parentType);
            return typeBuilder.CreateType();
        }

        static GenericTypeParameterBuilder[] SetupGenericArguments(Type[] genericParameterTypes,
                                                                   DefineGenericParametersDelegate @delegate)
        {
            // Nothing to do if it's not generic
            if (genericParameterTypes.Length == 0)
                return null;

            // Extract parameter names
            string[] genericParameterNames = new string[genericParameterTypes.Length];
            for (int idx = 0; idx < genericParameterTypes.Length; idx++)
                genericParameterNames[idx] = genericParameterTypes[idx].Name;

            // Setup constraints on generic types (i.e., "where" clauses)
            GenericTypeParameterBuilder[] genericBuilders = @delegate(genericParameterNames);

            for (int idx = 0; idx < genericBuilders.Length; idx++)
            {
                genericBuilders[idx].SetGenericParameterAttributes(genericParameterTypes[idx].GenericParameterAttributes);

                foreach (Type type in genericParameterTypes[idx].GetGenericParameterConstraints())
                    genericBuilders[idx].SetBaseTypeConstraint(type);
            }

            return genericBuilders;
        }

        static Type SetupGenericClassArguments(Type classToWrap,
                                               TypeBuilder typeBuilder)
        {
            GenericTypeParameterBuilder[] builders =
                SetupGenericArguments(classToWrap.GetGenericArguments(),
                                      delegate(string[] names)
                                      {
                                          return typeBuilder.DefineGenericParameters(names);
                                      });

            if (builders != null)
                return classToWrap.MakeGenericType(builders);

            return classToWrap;
        }

        static Type[] SetupGenericMethodArguments(MethodBase methodToIntercept,
                                                  MethodBuilder methodBuilder)
        {
            Type[] arguments = methodToIntercept.GetGenericArguments();
            SetupGenericArguments(arguments,
                                  delegate(string[] names)
                                  {
                                      return methodBuilder.DefineGenericParameters(names);
                                  });
            return arguments;
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
                    AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
                    ModuleBuilder module = assemblyBuilder.DefineDynamicModule(assemblyName + ".dll");
                    wrappers[actualClassToWrap] = GenerateWrapperType(actualClassToWrap, module);
                }

                if (actualClassToWrap != classToWrap)
                    return wrappers[actualClassToWrap].MakeGenericType(classToWrap.GetGenericArguments());

                return wrappers[classToWrap];
            }
        }

        delegate GenericTypeParameterBuilder[] DefineGenericParametersDelegate(string[] parameterNames);
    }
}