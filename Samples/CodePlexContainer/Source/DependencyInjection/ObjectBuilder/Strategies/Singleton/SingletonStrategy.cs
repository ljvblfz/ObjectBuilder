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

            if (context.Locator != null && context.Locator.Contains(key))
                return context.Locator.Get(key);

            existing = base.BuildUp(context, typeToBuild, existing, idToBuild);

            if (context.Locator != null && context.Lifetime != null)
            {
                ISingletonPolicy singletonPolicy = context.Policies.Get<ISingletonPolicy>(typeToBuild, idToBuild);

                if (singletonPolicy != null && singletonPolicy.IsSingleton)
                {
                    lock (context.Locator)
                        context.Locator.Add(key, existing);

                    lock (context.Lifetime)
                        context.Lifetime.Add(existing);
                }
            }

            return existing;
        }
    }
}