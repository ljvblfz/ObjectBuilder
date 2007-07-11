using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualMethodInterceptionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            ICreationPolicy creationPolicy = context.Policies.Get<ICreationPolicy>(typeToBuild, idToBuild);
            VirtualInterceptionPolicy interceptionPolicy =
                context.Policies.Get<IInterceptionPolicy>(typeToBuild, idToBuild) as VirtualInterceptionPolicy;

            if (creationPolicy != null && interceptionPolicy != null)
            {
                ConstructorInfo ctor = creationPolicy.SelectConstructor(context, typeToBuild, idToBuild);
                object[] ctorParams = creationPolicy.GetParameters(context, typeToBuild, idToBuild, ctor);

                if (context.OriginalType.IsInterface)
                    typeToBuild = InterceptInterface(context, typeToBuild, idToBuild, interceptionPolicy, ctor, ctorParams);
                else
                    typeToBuild = InterceptClass(context, typeToBuild, idToBuild, interceptionPolicy, ctorParams);
            }

            return base.BuildUp(context, typeToBuild, existing, context.OriginalID);
        }

        static Type InterceptClass(IBuilderContext context,
                                   Type typeToBuild,
                                   string idToBuild,
                                   IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers,
                                   object[] originalParameters)
        {
            // Create a wrapper class which derives from the intercepted class
            typeToBuild = VirtualInterceptor.WrapClass(typeToBuild);

            // Create the proxy that's used by the wrapper
            ILEmitProxy proxy = new ILEmitProxy(handlers);

            // Create a new policy which calls the proper constructor
            List<Type> newParameterTypes = new List<Type>();
            List<IParameter> newIParameters = new List<IParameter>();

            newParameterTypes.Add(typeof(ILEmitProxy));
            newIParameters.Add(new ValueParameter<ILEmitProxy>(proxy));

            foreach (object obj in originalParameters)
            {
                newParameterTypes.Add(obj.GetType());
                newIParameters.Add(new ValueParameter(obj.GetType(), obj));
            }

            ConstructorInfo newConstructor = typeToBuild.GetConstructor(newParameterTypes.ToArray());
            ConstructorPolicy newPolicy = new ConstructorPolicy(newConstructor, newIParameters.ToArray());

            context.Policies.Set<ICreationPolicy>(newPolicy, typeToBuild, idToBuild);

            // Return the wrapped type for building
            return typeToBuild;
        }

        static Type InterceptInterface(IBuilderContext context,
                                       Type typeToBuild,
                                       string idToBuild,
                                       IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers,
                                       ConstructorInfo ctor,
                                       object[] ctorParams)
        {
            // Create a wrapper class which implements the interface
            typeToBuild = InterfaceInterceptor.WrapInterface(context.OriginalType);

            // Create an instance of the concrete class using the policy data
            object wrappedObject = ctor.Invoke(ctorParams);

            // Create the proxy that's used by the wrapper
            ILEmitProxy proxy = new ILEmitProxy(handlers);

            // Create a new policy which calls the proper constructor
            ConstructorInfo newConstructor = typeToBuild.GetConstructor(new Type[] { typeof(ILEmitProxy), context.OriginalType });
            ConstructorPolicy newPolicy = new ConstructorPolicy(newConstructor,
                                                                new ValueParameter<ILEmitProxy>(proxy),
                                                                new ValueParameter(context.OriginalType, wrappedObject));

            context.Policies.Set<ICreationPolicy>(newPolicy, typeToBuild, idToBuild);

            // Return the wrapped type for building
            return typeToBuild;
        }
    }
}