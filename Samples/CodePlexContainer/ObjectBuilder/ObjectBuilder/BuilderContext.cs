namespace ObjectBuilder
{
    public class BuilderContext : IBuilderContext
    {
        readonly IStrategyChain chain;
        readonly ILifetimeContainer lifetime;
        readonly IReadWriteLocator locator;
        readonly object originalBuildKey;
        readonly IPolicyList policies;

        protected BuilderContext() {}

        public BuilderContext(IStrategyChain chain,
                              IReadWriteLocator locator,
                              ILifetimeContainer lifetime,
                              IPolicyList policies,
                              object originalBuildKey)
        {
            this.chain = chain;
            this.locator = locator;
            this.lifetime = lifetime;
            this.originalBuildKey = originalBuildKey;
            this.policies = new PolicyList(policies);
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

        public object OriginalBuildKey
        {
            get { return originalBuildKey; }
        }

        public IPolicyList Policies
        {
            get { return policies; }
        }

        public IBuilderStrategy GetNextInChain(IBuilderStrategy currentStrategy)
        {
            return chain.GetNext(currentStrategy);
        }
    }
}