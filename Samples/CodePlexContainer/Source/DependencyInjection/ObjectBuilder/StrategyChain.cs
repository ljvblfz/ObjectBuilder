using System.Collections;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class StrategyChain : IStrategyChain
    {
        // Fields

        List<IBuilderStrategy> strategies = new List<IBuilderStrategy>();

        // Lifetime

        public StrategyChain() {}

        public StrategyChain(IEnumerable<IBuilderStrategy> strategies)
        {
            AddRange(strategies);
        }

        // Properties

        public IBuilderStrategy Head
        {
            get
            {
                if (strategies.Count > 0)
                    return strategies[0];
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

        public IStrategyChain Reverse()
        {
            List<IBuilderStrategy> reverseList = new List<IBuilderStrategy>(strategies);
            reverseList.Reverse();
            return new StrategyChain(reverseList);
        }
    }
}