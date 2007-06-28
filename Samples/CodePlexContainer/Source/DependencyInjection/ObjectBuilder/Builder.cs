using System;
using CodePlex.DependencyInjection.Properties;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class Builder : IBuilder
    {
        public object BuildUp(IReadWriteLocator locator,
                              ILifetimeContainer lifetime,
                              PolicyList policies,
                              IStrategyChain strategies,
                              Type typeToBuild,
                              string idToBuild,
                              object existing)
        {
            Guard.ArgumentNotNull(strategies, "strategies");
            GuardStrategiesNotEmpty(strategies);

            BuilderContext context = new BuilderContext(strategies, locator, lifetime, policies, typeToBuild, idToBuild);
            return context.HeadOfChain.BuildUp(context, typeToBuild, existing, idToBuild);
        }

        public TTypeToBuild BuildUp<TTypeToBuild>(IReadWriteLocator locator,
                                                  ILifetimeContainer lifetime,
                                                  PolicyList policies,
                                                  IStrategyChain strategies,
                                                  string idToBuild,
                                                  object existing)
        {
            return (TTypeToBuild)BuildUp(locator, lifetime, policies, strategies, typeof(TTypeToBuild), idToBuild, existing);
        }

        static void GuardStrategiesNotEmpty(IStrategyChain strategies)
        {
            if (strategies.Head == null)
                throw new ArgumentException(Resources.BuilderHasNoStrategies, "strategies");
        }

        public TItem TearDown<TItem>(IReadWriteLocator locator,
                                     ILifetimeContainer lifetime,
                                     PolicyList policies,
                                     IStrategyChain strategies,
                                     TItem item)
        {
            Guard.ArgumentNotNull(item, "item");
            Guard.ArgumentNotNull(strategies, "strategies");
            GuardStrategiesNotEmpty(strategies);

            BuilderContext context = new BuilderContext(strategies.Reverse(), locator, lifetime, policies, null, null);
            return (TItem)context.HeadOfChain.TearDown(context, item);
        }
    }
}