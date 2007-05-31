using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuilder<TStageEnum>
    {
        // Properties

        //[Obsolete]
        PolicyList Policies { get; }

        //[Obsolete]
        StrategyList<TStageEnum> Strategies { get; }

        // Methods

        //[Obsolete]
        object BuildUp(IReadWriteLocator locator,
                       Type typeToBuild,
                       string idToBuild,
                       object existing,
                       params PolicyList[] transientPolicies);

        //[Obsolete]
        TTypeToBuild BuildUp<TTypeToBuild>(IReadWriteLocator locator,
                                           string idToBuild,
                                           object existing,
                                           params PolicyList[] transientPolicies);

        //[Obsolete]
        TItem TearDown<TItem>(IReadWriteLocator locator,
                              TItem item);

        // New

        object BuildUp(IReadWriteLocator locator,
                       ILifetimeContainer lifetime,
                       PolicyList policies,
                       StrategyList<TStageEnum> strategies,
                       Type typeToBuild,
                       string idToBuild,
                       object existing);

        TTypeToBuild BuildUp<TTypeToBuild>(IReadWriteLocator locator,
                                           ILifetimeContainer lifetime,
                                           PolicyList policies,
                                           StrategyList<TStageEnum> strategies,
                                           string idToBuild,
                                           object existing);

        TItem TearDown<TItem>(IReadWriteLocator locator,
                              ILifetimeContainer lifetime,
                              PolicyList policies,
                              StrategyList<TStageEnum> strategies,
                              TItem item);
    }
}