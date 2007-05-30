using System;
using CodePlex.DependencyInjection.Properties;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class BuilderBase<TStageEnum> : IBuilder<TStageEnum>
    {
        // Fields

        PolicyList policies = new PolicyList();
        StrategyList<TStageEnum> strategies = new StrategyList<TStageEnum>();

        // Lifetime

        public BuilderBase() {}

        public BuilderBase(IBuilderConfigurator<TStageEnum> configurator)
        {
            configurator.ApplyConfiguration(this);
        }

        // Properties

        public PolicyList Policies
        {
            get { return policies; }
        }

        public StrategyList<TStageEnum> Strategies
        {
            get { return strategies; }
        }

        // Methods

        public TTypeToBuild BuildUp<TTypeToBuild>(IReadWriteLocator locator,
                                                  string idToBuild,
                                                  object existing,
                                                  params PolicyList[] transientPolicies)
        {
            return (TTypeToBuild)BuildUp(locator, typeof(TTypeToBuild), idToBuild, existing, transientPolicies);
        }

        public virtual object BuildUp(IReadWriteLocator locator,
                                      Type typeToBuild,
                                      string idToBuild,
                                      object existing,
                                      params PolicyList[] transientPolicies)
        {
            return DoBuildUp(locator, typeToBuild, idToBuild, existing, transientPolicies);
        }

        object DoBuildUp(IReadWriteLocator locator,
                         Type typeToBuild,
                         string idToBuild,
                         object existing,
                         PolicyList[] transientPolicies)
        {
            IBuilderStrategyChain chain = strategies.MakeStrategyChain();
            ThrowIfNoStrategiesInChain(chain);

            IBuilderContext context = MakeContext(chain, locator, transientPolicies);

            return chain.Head.BuildUp(context, typeToBuild, existing, idToBuild);
        }

        TItem DoTearDown<TItem>(IReadWriteLocator locator,
                                TItem item)
        {
            IBuilderStrategyChain chain = strategies.MakeReverseStrategyChain();
            ThrowIfNoStrategiesInChain(chain);

            Type type = item.GetType();
            IBuilderContext context = MakeContext(chain, locator);

            TItem result = (TItem)chain.Head.TearDown(context, item);

            return result;
        }

        IBuilderContext MakeContext(IBuilderStrategyChain chain,
                                    IReadWriteLocator locator,
                                    params PolicyList[] transientPolicies)
        {
            PolicyList policies = new PolicyList(this.policies);

            foreach (PolicyList policyList in transientPolicies)
                policies.AddPolicies(policyList);

            return new BuilderContext(chain, locator, policies);
        }

        public TItem TearDown<TItem>(IReadWriteLocator locator,
                                     TItem item)
        {
            if (typeof(TItem).IsValueType == false && item == null)
                throw new ArgumentNullException("item");

            return DoTearDown(locator, item);
        }

        static void ThrowIfNoStrategiesInChain(IBuilderStrategyChain chain)
        {
            if (chain.Head == null)
                throw new InvalidOperationException(Resources.BuilderHasNoStrategies);
        }
    }
}