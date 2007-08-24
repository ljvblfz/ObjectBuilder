namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class Builder : IBuilder
    {
        public object BuildUp(IReadWriteLocator locator,
                              ILifetimeContainer lifetime,
                              PolicyList policies,
                              IStrategyChain strategies,
                              object buildKey,
                              object existing)
        {
            Guard.ArgumentNotNull(strategies, "strategies");
            Guard.ArgumentNotNull(strategies.Head, "strategies.Head");

            BuilderContext context = new BuilderContext(strategies, locator, lifetime, policies, buildKey);
            return context.HeadOfChain.BuildUp(context, buildKey, existing);
        }

        public TTypeToBuild BuildUp<TTypeToBuild>(IReadWriteLocator locator,
                                                  ILifetimeContainer lifetime,
                                                  PolicyList policies,
                                                  IStrategyChain strategies,
                                                  object buildKey,
                                                  object existing)
        {
            return (TTypeToBuild)BuildUp(locator, lifetime, policies, strategies, buildKey, existing);
        }

        public TItem TearDown<TItem>(IReadWriteLocator locator,
                                     ILifetimeContainer lifetime,
                                     PolicyList policies,
                                     IStrategyChain strategies,
                                     TItem item)
        {
            Guard.ArgumentNotNull(item, "item");
            Guard.ArgumentNotNull(strategies, "strategies");
            Guard.ArgumentNotNull(strategies.Head, "strategies.Head");

            BuilderContext context = new BuilderContext(strategies.Reverse(), locator, lifetime, policies, null);
            return (TItem)context.HeadOfChain.TearDown(context, item);
        }
    }
}