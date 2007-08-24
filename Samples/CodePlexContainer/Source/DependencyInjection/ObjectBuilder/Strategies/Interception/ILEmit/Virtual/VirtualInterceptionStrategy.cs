using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualInterceptionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            ICreationPolicy creationPolicy = context.Policies.Get<ICreationPolicy>(buildKey);
            VirtualInterceptionPolicy interceptionPolicy = context.Policies.Get<VirtualInterceptionPolicy>(buildKey);
            Type typeToBuild;

            if (creationPolicy != null &&
                creationPolicy.SupportsReflection &&
                interceptionPolicy != null &&
                TryGetTypeFromBuildKey(buildKey, out typeToBuild))
            {
                ConstructorInfo ctor = creationPolicy.GetConstructor(context, buildKey);
                object[] ctorParams = creationPolicy.GetParameters(context, ctor);

                buildKey = InterceptClass(context, typeToBuild, interceptionPolicy, ctorParams);
            }

            return base.BuildUp(context, buildKey, existing);
        }

        static Type InterceptClass(IBuilderContext context,
                                   Type typeToBuild,
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
            ConstructorCreationPolicy newPolicy = new ConstructorCreationPolicy(newConstructor, newIParameters.ToArray());

            context.Policies.Set<ICreationPolicy>(newPolicy, typeToBuild);

            // Return the wrapped type for building
            return typeToBuild;
        }
    }
}