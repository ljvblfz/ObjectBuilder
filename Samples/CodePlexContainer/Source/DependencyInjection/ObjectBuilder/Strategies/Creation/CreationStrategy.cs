using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class CreationStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            return base.BuildUp(context, buildKey, existing ?? BuildUpNewObject(context, buildKey));
        }

        static object BuildUpNewObject(IBuilderContext context,
                                       object buildKey)
        {
            ICreationPolicy policy = context.Policies.Get<ICreationPolicy>(buildKey);

            if (policy == null)
                throw new InvalidOperationException("Could not find creation policy for build key " + buildKey);

            return policy.Create(context, buildKey);
        }
    }
}