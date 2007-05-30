using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class StrategyList<TStageEnum>
    {
        // Fields

        static readonly Array stageValues = Enum.GetValues(typeof(TStageEnum));
        Dictionary<TStageEnum, List<IBuilderStrategy>> stages;
        object lockObject = new object();

        // Lifetime

        public StrategyList()
        {
            stages = new Dictionary<TStageEnum, List<IBuilderStrategy>>();

            foreach (TStageEnum stage in stageValues)
                stages[stage] = new List<IBuilderStrategy>();
        }

        // Methods

        public void Add(IBuilderStrategy strategy,
                        TStageEnum stage)
        {
            lock (lockObject)
                stages[stage].Add(strategy);
        }

        public void AddNew<TStrategy>(TStageEnum stage)
            where TStrategy : IBuilderStrategy, new()
        {
            lock (lockObject)
                stages[stage].Add(new TStrategy());
        }

        public void Clear()
        {
            lock (lockObject)
                foreach (TStageEnum stage in stageValues)
                    stages[stage].Clear();
        }

        public IBuilderStrategyChain MakeReverseStrategyChain()
        {
            lock (lockObject)
            {
                List<IBuilderStrategy> tempList = new List<IBuilderStrategy>();
                foreach (TStageEnum stage in stageValues)
                    tempList.AddRange(stages[stage]);

                tempList.Reverse();

                BuilderStrategyChain result = new BuilderStrategyChain();
                result.AddRange(tempList);
                return result;
            }
        }

        public IBuilderStrategyChain MakeStrategyChain()
        {
            lock (lockObject)
            {
                BuilderStrategyChain result = new BuilderStrategyChain();

                foreach (TStageEnum stage in stageValues)
                    result.AddRange(stages[stage]);

                return result;
            }
        }
    }
}