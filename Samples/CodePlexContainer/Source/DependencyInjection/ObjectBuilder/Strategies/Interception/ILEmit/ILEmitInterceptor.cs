using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class ILEmitInterceptor
    {
        static readonly AssemblyBuilder assemblyBuilder;
        static readonly Dictionary<Type, Type> wrappers = new Dictionary<Type, Type>();

        static ILEmitInterceptor()
        {
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ILEmitDynamicTypes"), AssemblyBuilderAccess.RunAndSave);
        }

        protected abstract Type GenerateWrapperType(Type typeToWrap,
                                                    ModuleBuilder moduleBuilder,
                                                    string moduleName);

        protected static string MakeTypeName(string assemblyName,
                                             Type typeToWrap)
        {
            return "{Wrappers}.ns" + assemblyName + "." + typeToWrap.Name;
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

        protected static Type SetupGenericClassArguments(Type classToWrap,
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

        protected static Type[] SetupGenericMethodArguments(MethodBase methodToIntercept,
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

        protected Type Wrap(Type typeToWrap)
        {
            lock (wrappers)
            {
                Type actualClassToWrap = typeToWrap;

                if (typeToWrap.IsGenericType)
                    actualClassToWrap = typeToWrap.GetGenericTypeDefinition();

                if (!wrappers.ContainsKey(actualClassToWrap))
                {
                    string moduleName = Guid.NewGuid().ToString("N");
                    ModuleBuilder module = assemblyBuilder.DefineDynamicModule(moduleName + ".dll");
                    wrappers[actualClassToWrap] = GenerateWrapperType(actualClassToWrap, module, moduleName);
                }

                if (actualClassToWrap != typeToWrap)
                    return wrappers[actualClassToWrap].MakeGenericType(typeToWrap.GetGenericArguments());

                return wrappers[typeToWrap];
            }
        }

        delegate GenericTypeParameterBuilder[] DefineGenericParametersDelegate(string[] parameterNames);
    }
}