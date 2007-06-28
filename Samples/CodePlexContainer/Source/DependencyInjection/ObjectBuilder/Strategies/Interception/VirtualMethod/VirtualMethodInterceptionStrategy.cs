using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class VirtualMethodInterceptionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            IInterceptionPolicy policy = context.Policies.Get<IInterceptionPolicy>(typeToBuild, idToBuild);

            if (existing != null && policy != null && policy.InterceptionType == InterceptionType.VirtualMethod)
                existing = VirtualMethodInterceptor.Wrap(existing, policy);

            return base.BuildUp(context, context.OriginalType, existing, context.OriginalID);
        }
    }
}