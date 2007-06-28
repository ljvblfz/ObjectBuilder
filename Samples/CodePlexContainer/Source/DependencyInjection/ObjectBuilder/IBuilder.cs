using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuilder
    {
        object BuildUp(IReadWriteLocator locator,
                       ILifetimeContainer lifetime,
                       PolicyList policies,
                       IStrategyChain strategies,
                       Type typeToBuild,
                       string idToBuild,
                       object existing);

        TTypeToBuild BuildUp<TTypeToBuild>(IReadWriteLocator locator,
                                           ILifetimeContainer lifetime,
                                           PolicyList policies,
                                           IStrategyChain strategies,
                                           string idToBuild,
                                           object existing);

        TItem TearDown<TItem>(IReadWriteLocator locator,
                              ILifetimeContainer lifetime,
                              PolicyList policies,
                              IStrategyChain strategies,
                              TItem item);
    }
}