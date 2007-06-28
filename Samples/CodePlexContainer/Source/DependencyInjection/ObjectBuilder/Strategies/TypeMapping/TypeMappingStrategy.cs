using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class TypeMappingStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type t,
                                       object existing,
                                       string id)
        {
            DependencyResolutionLocatorKey result = new DependencyResolutionLocatorKey(t, id);
            ITypeMappingPolicy policy = context.Policies.Get<ITypeMappingPolicy>(t, id);

            if (policy != null)
            {
                result = policy.Map(result);
                Guard.TypeIsAssignableFromType(t, result.Type, t);
            }

            return base.BuildUp(context, result.Type, existing, result.ID);
        }
    }
}