namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IStrategyChain
    {
        // Properties

        IBuilderStrategy Head { get; }

        // Methods

        IBuilderStrategy GetNext(IBuilderStrategy currentStrategy);
        IStrategyChain Reverse();
    }
}