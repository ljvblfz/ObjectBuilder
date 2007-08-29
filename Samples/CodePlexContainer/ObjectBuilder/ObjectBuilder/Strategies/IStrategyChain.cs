namespace ObjectBuilder
{
    public interface IStrategyChain
    {
        IBuilderStrategy Head { get; }

        IBuilderStrategy GetNext(IBuilderStrategy currentStrategy);
        IStrategyChain Reverse();
    }
}