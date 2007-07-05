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
            VirtualMethodInterceptionPolicy interceptionPolicy =
                context.Policies.Get<IInterceptionPolicy>(typeToBuild, idToBuild) as VirtualMethodInterceptionPolicy;

            if (creationPolicy != null && interceptionPolicy != null)
                //if (context.OriginalType.IsInterface)
                //    throw new NotImplementedException("Want new implementation specifically for interfaces");
                //else
                    typeToBuild = InterceptClass(context, typeToBuild, idToBuild, creationPolicy, interceptionPolicy);

            return base.BuildUp(context, typeToBuild, existing, context.OriginalID);
        }

        static Type InterceptClass(IBuilderContext context,
                                   Type typeToBuild,
                                   string idToBuild,
                                   ICreationPolicy creationPolicy,
                                   IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers)
        {
            ConstructorInfo originalConstructor = creationPolicy.SelectConstructor(context, typeToBuild, idToBuild);
            object[] originalParameters = creationPolicy.GetParameters(context, typeToBuild, idToBuild, originalConstructor);

            typeToBuild = VirtualMethodClassInterceptor.WrapClass(typeToBuild);

            VirtualMethodProxy proxy = new VirtualMethodProxy(handlers);
            List<Type> newParameterTypes = new List<Type>();
            List<IParameter> newIParameters = new List<IParameter>();

            newParameterTypes.Add(typeof(VirtualMethodProxy));
            newIParameters.Add(new ValueParameter<VirtualMethodProxy>(proxy));

            foreach (object obj in originalParameters)
            {
                newParameterTypes.Add(obj.GetType());
                newIParameters.Add(new ValueParameter(obj.GetType(), obj));
            }

            ConstructorInfo newConstructor = typeToBuild.GetConstructor(newParameterTypes.ToArray());
            ConstructorPolicy newPolicy = new ConstructorPolicy(newConstructor, newIParameters.ToArray());

            context.Policies.Set<ICreationPolicy>(newPolicy, typeToBuild, idToBuild);
            return typeToBuild;
        }
    }
}