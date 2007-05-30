using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuilder<TStageEnum>
    {
        // Properties

        PolicyList Policies { get; }
        StrategyList<TStageEnum> Strategies { get; }

        // Methods

        object BuildUp(IReadWriteLocator locator,
                       Type typeToBuild,
                       string idToBuild,
                       object existing,
                       params PolicyList[] transientPolicies);

        TTypeToBuild BuildUp<TTypeToBuild>(IReadWriteLocator locator,
                                           string idToBuild,
                                           object existing,
                                           params PolicyList[] transientPolicies);

        TItem TearDown<TItem>(IReadWriteLocator locator,
                              TItem item);
    }
}