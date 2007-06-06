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
            IInterceptionPolicy policy = context.Policies.Get<IInterceptionPolicy>(typeToBuild, idToBuild);

            if (existing != null && policy != null && policy.InterceptionType == InterceptionType.Remoting)
                existing = RemotingInterceptor.Wrap(existing, typeToBuild, policy);

            return existing;
        }
    }
}