using CodePlex.DependencyInjection.ObjectBuilder;

namespace CodePlex.DependencyInjection
{
    class MockBuilderContext : IBuilderContext
    {
        readonly ILifetimeContainer lifetime = new LifetimeContainer();
        readonly IReadWriteLocator locator;
        readonly object originalBuildKey = null;
        readonly IPolicyList policies = new PolicyList();
        readonly StrategyChain strategies = new StrategyChain();

        public MockBuilderContext()
            : this(new Locator()) {}

        public MockBuilderContext(IReadWriteLocator locator)
        {
            this.locator = locator;
        }

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

        public object OriginalBuildKey
        {
            get { return originalBuildKey; }
        }

        public IPolicyList Policies
        {
            get { return policies; }
        }

        public StrategyChain Strategies
        {
            get { return strategies; }
        }

        public IBuilderStrategy GetNextInChain(IBuilderStrategy currentStrategy)
        {
            return strategies.GetNext(currentStrategy);
        }
    }
}