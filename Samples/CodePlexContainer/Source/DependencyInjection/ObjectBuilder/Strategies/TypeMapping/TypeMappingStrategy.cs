using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class TypeMappingStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            ITypeMappingPolicy policy = context.Policies.Get<ITypeMappingPolicy>(typeToBuild, idToBuild);

            if (policy != null)
            {
                DependencyResolutionLocatorKey resolution = policy.Map(new DependencyResolutionLocatorKey(typeToBuild, idToBuild));

                if (resolution.Type.IsGenericType)
                    typeToBuild = resolution.Type.MakeGenericType(typeToBuild.GetGenericArguments());
                else
                    typeToBuild = resolution.Type;

                idToBuild = resolution.ID;
            }

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }
    }
}