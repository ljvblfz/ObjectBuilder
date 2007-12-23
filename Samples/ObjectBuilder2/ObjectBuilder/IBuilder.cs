namespace ObjectBuilder
{
    public interface IBuilder
    {
        object BuildUp(IReadWriteLocator locator,
                       ILifetimeContainer lifetime,
                       IPolicyList policies,
                       IStrategyChain strategies,
                       object buildKey,
                       object existing);

        TTypeToBuild BuildUp<TTypeToBuild>(IReadWriteLocator locator,
                                           ILifetimeContainer lifetime,
                                           IPolicyList policies,
                                           IStrategyChain strategies,
                                           object buildKey,
                                           object existing);

        TItem TearDown<TItem>(IReadWriteLocator locator,
                              ILifetimeContainer lifetime,
                              IPolicyList policies,
                              IStrategyChain strategies,
                              TItem item);
    }
}