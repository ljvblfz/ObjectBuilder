using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class RemotingInterceptionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            Type typeToBuild;

            if (TryGetTypeFromBuildKey(buildKey, out typeToBuild))
            {
                Type originalType;
                if (!TryGetTypeFromBuildKey(context.OriginalBuildKey, out originalType))
                    originalType = typeToBuild;

                RemotingInterceptionPolicy policy = context.Policies.Get<RemotingInterceptionPolicy>(buildKey);

                if (existing != null && policy != null)
                    existing = RemotingInterceptor.Wrap(existing, originalType, policy);
            }

            return base.BuildUp(context, buildKey, existing);
        }
    }
}