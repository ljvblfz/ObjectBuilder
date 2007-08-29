using System;

namespace ObjectBuilder
{
    public class BuildKeyMappingStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            IBuildKeyMappingPolicy policy = context.Policies.Get<IBuildKeyMappingPolicy>(buildKey);

            if (policy != null)
            {
                object newBuildKey = policy.Map(buildKey);

                Type originalType;
                Type newType;

                if (TryGetTypeFromBuildKey(newBuildKey, out newType)
                    && TryGetTypeFromBuildKey(buildKey, out originalType)
                    && originalType.IsGenericType)
                    buildKey = newType.MakeGenericType(originalType.GetGenericArguments());
                else
                    buildKey = newType;
            }

            return base.BuildUp(context, buildKey, existing);

            /*
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
             */
        }
    }
}