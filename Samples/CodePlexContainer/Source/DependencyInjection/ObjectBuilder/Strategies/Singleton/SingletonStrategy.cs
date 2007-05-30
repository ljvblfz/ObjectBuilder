using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class SingletonStrategy : BuilderStrategy
    {
        // Methods

        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            DependencyResolutionLocatorKey key = new DependencyResolutionLocatorKey(typeToBuild, idToBuild);

            if (context.Locator != null && context.Locator.Contains(key, SearchMode.Up))
                return context.Locator.Get(key);

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }
    }
}