using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class BuilderContext : IBuilderContext
    {
        // Fields

        IStrategyChain chain;
        ILifetimeContainer lifetime;
        IReadWriteLocator locator;
        PolicyList policies;
        string originalID;
        Type originalType;

        // Lifetime

        protected BuilderContext() {}

        public BuilderContext(IStrategyChain chain,
                              IReadWriteLocator locator,
                              ILifetimeContainer lifetime,
                              PolicyList policies,
                              Type originalType,
                              string originalID)
        {
            this.chain = chain;
            this.locator = locator;
            this.lifetime = lifetime;
            this.policies = new PolicyList(policies);
            this.originalType = originalType;
            this.originalID = originalID;
        }

        // Properties

        public IBuilderStrategy HeadOfChain
        {
            get { return chain.Head; }
        }

        public ILifetimeContainer Lifetime
        {
            get { return lifetime; }
        }

        public IReadWriteLocator Locator
        {
            get { return locator; }
        }

        public string OriginalID
        {
            get { return originalID; }
        }

        public Type OriginalType
        {
            get { return originalType; }
        }

        public PolicyList Policies
        {
            get { return policies; }
        }

        // Methods

        public IBuilderStrategy GetNextInChain(IBuilderStrategy currentStrategy)
        {
            return chain.GetNext(currentStrategy);
        }
    }
}