using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class InterceptionReflectionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            IObjectFactory factory = (IObjectFactory)context.Locator.Get(new DependencyResolutionLocatorKey(typeof(IObjectFactory), null));

            if (factory != null)
                InterceptionReflector.Reflect(context.OriginalType, typeToBuild, idToBuild, context.Policies, factory);

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }
    }
}