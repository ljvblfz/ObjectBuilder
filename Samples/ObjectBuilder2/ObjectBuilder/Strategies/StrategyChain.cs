using System.Collections;
using System.Collections.Generic;

namespace ObjectBuilder
{
    public class StrategyChain : IStrategyChain
    {
        readonly List<IBuilderStrategy> strategies = new List<IBuilderStrategy>();

        public StrategyChain() {}

        public StrategyChain(IEnumerable strategies)
        {
            AddRange(strategies);
        }

        public IBuilderStrategy Head
        {
            get
            {
                if (strategies.Count > 0)
                    return strategies[0];
                return null;
            }
        }

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