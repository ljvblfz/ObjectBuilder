using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    class MockBuilderContext : IBuilderContext
    {
        // Fields

        ILifetimeContainer lifetime = new LifetimeContainer();
        IReadWriteLocator locator;
        string originalID;
        Type originalType;
        PolicyList policies = new PolicyList();
        StrategyChain strategies = new StrategyChain();

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

        public string OriginalID
        {
            get { return originalID; }
            set { originalID = value; }
        }

        public Type OriginalType
        {
            get { return originalType; }
            set { originalType = value; }
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