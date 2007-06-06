using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class LifetimeStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            object obj = base.BuildUp(context, typeToBuild, existing, idToBuild);

            if (context.Lifetime != null)
                context.Lifetime.Add(obj);

            return obj;
        }
    }
}