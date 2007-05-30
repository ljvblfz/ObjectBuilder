using System.Collections;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class BuilderStrategyChain : IBuilderStrategyChain
    {
        // Fields

        List<IBuilderStrategy> strategies;

        // Lifetime

        public BuilderStrategyChain()
        {
            strategies = new List<IBuilderStrategy>();
        }

        // Properties

        public IBuilderStrategy Head
        {
            get
            {
                if (strategies.Count > 0)
                    return strategies[0];
                else
                    return null;
            }
        }

        // Methods

        public void Add(IBuilderStrategy strategy)
        {
            strategies.Add(strategy);
        }

        public void AddRange(IEnumerable strategyEnumerable)
        {
            foreach (IBuilderStrategy strategy in strategyEnumerable)
                Add(strategy);
        }

        public IBuilderStrategy GetNext(IBuilderStrategy currentStrategy)
        {
            for (int idx = 0; idx < strategies.Count - 1; idx++)
                if (ReferenceEquals(currentStrategy, strategies[idx]))
                    return strategies[idx + 1];

            return null;
        }
    }
}