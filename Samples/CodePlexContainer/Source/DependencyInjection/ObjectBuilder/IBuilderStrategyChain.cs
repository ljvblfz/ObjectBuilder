using System.Collections;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuilderStrategyChain
    {
        // Properties

        IBuilderStrategy Head { get; }

        // Methods

        void Add(IBuilderStrategy strategy);
        void AddRange(IEnumerable strategyEnumerable);
        IBuilderStrategy GetNext(IBuilderStrategy currentStrategy);
    }
}