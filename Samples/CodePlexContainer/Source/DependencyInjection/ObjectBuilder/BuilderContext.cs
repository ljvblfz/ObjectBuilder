namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class BuilderContext : IBuilderContext
    {
        // Fields

        IStrategyChain chain;
        ILifetimeContainer lifetime;
        IReadWriteLocator locator;
        PolicyList policies;

        // Lifetime

        protected BuilderContext() {}

        public BuilderContext(IStrategyChain chain,
                              IReadWriteLocator locator,
                              ILifetimeContainer lifetime,
                              PolicyList policies)
        {
            this.chain = chain;
            this.locator = locator;
            this.lifetime = lifetime;
            this.policies = new PolicyList(policies);
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