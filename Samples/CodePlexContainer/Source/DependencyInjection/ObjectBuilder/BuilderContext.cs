namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class BuilderContext : IBuilderContext
    {
        // Fields

        IBuilderStrategyChain chain;
        IReadWriteLocator locator;
        PolicyList policies;

        // Lifetime

        protected BuilderContext() {}

        public BuilderContext(IBuilderStrategyChain chain,
                              IReadWriteLocator locator,
                              PolicyList policies)
        {
            this.chain = chain;
            this.locator = locator;
            this.policies = new PolicyList(policies);
        }

        // Properties

        public IBuilderStrategy HeadOfChain
        {
            get { return chain.Head; }
        }

        public IReadWriteLocator Locator
        {
            get { return locator; }
        }

        public PolicyList Policies
        {
            get { return policies; }
        }

        protected IBuilderStrategyChain StrategyChain
        {
            get { return chain; }
            set { chain = value; }
        }

        // Methods

        public IBuilderStrategy GetNextInChain(IBuilderStrategy currentStrategy)
        {
            return chain.GetNext(currentStrategy);
        }

        protected void SetLocator(IReadWriteLocator locator)
        {
            this.locator = locator;
        }

        protected void SetPolicies(PolicyList policies)
        {
            this.policies = policies;
        }
    }
}