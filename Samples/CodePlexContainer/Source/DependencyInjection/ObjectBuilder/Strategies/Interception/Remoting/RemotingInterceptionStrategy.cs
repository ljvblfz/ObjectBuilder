using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class RemotingInterceptionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            RemotingInterceptionPolicy policy =
                context.Policies.Get<IInterceptionPolicy>(typeToBuild, idToBuild) as RemotingInterceptionPolicy;

            if (existing != null && policy != null)
                existing = RemotingInterceptor.Wrap(existing, context.OriginalType, policy);

            return base.BuildUp(context, context.OriginalType, existing, context.OriginalID);
        }
    }
}