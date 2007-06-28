using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class BuilderContext : IBuilderContext
    {
        readonly IStrategyChain chain;
        readonly ILifetimeContainer lifetime;
        readonly IReadWriteLocator locator;
        readonly string originalID;
        readonly Type originalType;
        readonly PolicyList policies;

        protected BuilderContext() {}

        public BuilderContext(IStrategyChain chain,
                              IReadWriteLocator locator,
                              ILifetimeContainer lifetime,
                              IPolicyList policies,
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

        public IBuilderStrategy GetNextInChain(IBuilderStrategy currentStrategy)
        {
            return chain.GetNext(currentStrategy);
        }
    }
}