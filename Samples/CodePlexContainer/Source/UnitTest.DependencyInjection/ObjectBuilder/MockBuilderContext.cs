namespace CodePlex.DependencyInjection.ObjectBuilder
{
    class MockBuilderContext : BuilderContext
    {
        public IReadWriteLocator InnerLocator;
        public BuilderStrategyChain InnerChain = new BuilderStrategyChain();
        public PolicyList InnerPolicies = new PolicyList();
        public LifetimeContainer lifetimeContainer = new LifetimeContainer();

        public MockBuilderContext()
            : this(new Locator()) {}

        public MockBuilderContext(IReadWriteLocator locator)
        {
            InnerLocator = locator;
            SetLocator(InnerLocator);
            StrategyChain = InnerChain;
            SetPolicies(InnerPolicies);

            if (!Locator.Contains(typeof(ILifetimeContainer)))
                Locator.Add(typeof(ILifetimeContainer), lifetimeContainer);
        }
    }
}