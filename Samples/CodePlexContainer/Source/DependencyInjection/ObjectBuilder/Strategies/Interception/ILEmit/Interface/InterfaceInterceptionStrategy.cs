using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class InterfaceInterceptionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            ICreationPolicy creationPolicy = context.Policies.Get<ICreationPolicy>(typeToBuild, idToBuild);
            InterfaceInterceptionPolicy interceptionPolicy = context.Policies.Get<InterfaceInterceptionPolicy>(typeToBuild, idToBuild);

            if (creationPolicy != null && interceptionPolicy != null)
            {
                ConstructorInfo ctor = creationPolicy.SelectConstructor(context, typeToBuild, idToBuild);
                object[] ctorParams = creationPolicy.GetParameters(context, typeToBuild, idToBuild, ctor);

                typeToBuild = InterceptInterface(context, typeToBuild, idToBuild, interceptionPolicy, ctor, ctorParams);
            }

            return base.BuildUp(context, typeToBuild, existing, context.OriginalID);
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