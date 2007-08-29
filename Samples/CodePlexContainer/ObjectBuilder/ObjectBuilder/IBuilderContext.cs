namespace ObjectBuilder
{
    public interface IBuilderContext
    {
        IBuilderStrategy HeadOfChain { get; }
        ILifetimeContainer Lifetime { get; }
        IReadWriteLocator Locator { get; }
        object OriginalBuildKey { get; }
        IPolicyList Policies { get; }

        IBuilderStrategy GetNextInChain(IBuilderStrategy currentStrategy);
    }
}