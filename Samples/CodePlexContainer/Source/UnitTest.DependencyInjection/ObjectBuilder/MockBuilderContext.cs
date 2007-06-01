namespace CodePlex.DependencyInjection.ObjectBuilder
{
    class MockBuilderContext : IBuilderContext
    {
        // Fields

        StrategyChain strategies = new StrategyChain();
        ILifetimeContainer lifetime = new LifetimeContainer();
        IReadWriteLocator locator;
        PolicyList policies = new PolicyList();

        // Lifetime

        public MockBuilderContext()
            : this(new Locator()) {}

        public MockBuilderContext(IReadWriteLocator locator)
        {
            this.locator = locator;
        }

        // Properties

        public IBuilderStrategy HeadOfChain
        {
            get { return strategies.Head; }
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

        public StrategyChain Strategies
        {
            get { return strategies; }
        }

        // Methods

        public IBuilderStrategy GetNextInChain(IBuilderStrategy currentStrategy)
        {
            return strategies.GetNext(currentStrategy);
        }
    }
}