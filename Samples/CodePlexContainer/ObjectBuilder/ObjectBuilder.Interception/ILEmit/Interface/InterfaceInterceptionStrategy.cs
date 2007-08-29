using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public class InterfaceInterceptionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            ICreationPolicy creationPolicy = context.Policies.Get<ICreationPolicy>(buildKey);
            InterfaceInterceptionPolicy interceptionPolicy = context.Policies.Get<InterfaceInterceptionPolicy>(buildKey);
            Type typeToBuild;

            if (creationPolicy != null &&
                creationPolicy.SupportsReflection &&
                interceptionPolicy != null &&
                TryGetTypeFromBuildKey(buildKey, out typeToBuild))
            {
                ConstructorInfo ctor = creationPolicy.GetConstructor(context, buildKey);
                object[] ctorParams = creationPolicy.GetParameters(context, ctor);
                Type originalType;

                if (!TryGetTypeFromBuildKey(context.OriginalBuildKey, out originalType))
                    originalType = typeToBuild;

                buildKey = InterceptInterface(context, typeToBuild, originalType, interceptionPolicy, ctor, ctorParams);
            }

            return base.BuildUp(context, buildKey, existing);
        }

        static Type InterceptInterface(IBuilderContext context,
                                       Type typeToBuild,
                                       Type originalType,
                                       IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers,
                                       ConstructorInfo ctor,
                                       object[] ctorParams)
        {
            // Create a wrapper class which implements the interface
            typeToBuild = InterfaceInterceptor.WrapInterface(originalType);

            // Create an instance of the concrete class using the policy data
            object wrappedObject = ctor.Invoke(ctorParams);

            // Create the proxy that's used by the wrapper
            ILEmitProxy proxy = new ILEmitProxy(handlers);

            // Create a new policy which calls the proper constructor
            ConstructorInfo newConstructor = typeToBuild.GetConstructor(new Type[] { typeof(ILEmitProxy), originalType });
            ConstructorCreationPolicy newPolicy =
                new ConstructorCreationPolicy(newConstructor,
                                              new ValueParameter<ILEmitProxy>(proxy),
                                              new ValueParameter(originalType, wrappedObject));

            context.Policies.Set<ICreationPolicy>(newPolicy, typeToBuild);

            // Return the wrapped type for building
            return typeToBuild;
        }
    }
}