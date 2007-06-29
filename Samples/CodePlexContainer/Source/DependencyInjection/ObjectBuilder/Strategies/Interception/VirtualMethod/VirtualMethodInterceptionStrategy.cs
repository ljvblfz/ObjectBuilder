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
            IInterceptionPolicy interceptionPolicy = context.Policies.Get<IInterceptionPolicy>(typeToBuild, idToBuild);
            ICreationPolicy creationPolicy = context.Policies.Get<ICreationPolicy>(typeToBuild, idToBuild);

            if (creationPolicy != null &&
                interceptionPolicy != null &&
                interceptionPolicy.InterceptionType == InterceptionType.VirtualMethod)
            {
                ConstructorInfo originalConstructor = creationPolicy.SelectConstructor(context, typeToBuild, idToBuild);
                object[] originalParameters = creationPolicy.GetParameters(context, typeToBuild, idToBuild, originalConstructor);

                typeToBuild = VirtualMethodInterceptor.WrapType(typeToBuild);

                VirtualMethodProxy proxy = new VirtualMethodProxy(interceptionPolicy);
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
            }

            return base.BuildUp(context, typeToBuild, existing, context.OriginalID);
        }
    }
}