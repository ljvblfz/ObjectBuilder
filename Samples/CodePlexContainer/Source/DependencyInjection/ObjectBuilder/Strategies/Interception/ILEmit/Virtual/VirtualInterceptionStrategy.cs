using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualInterceptionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            ICreationPolicy creationPolicy = context.Policies.Get<ICreationPolicy>(typeToBuild, idToBuild);
            VirtualInterceptionPolicy interceptionPolicy = context.Policies.Get<VirtualInterceptionPolicy>(typeToBuild, idToBuild);

            if (creationPolicy != null && interceptionPolicy != null)
            {
                ConstructorInfo ctor = creationPolicy.SelectConstructor(context, typeToBuild, idToBuild);
                object[] ctorParams = creationPolicy.GetParameters(context, typeToBuild, idToBuild, ctor);

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
    }
}