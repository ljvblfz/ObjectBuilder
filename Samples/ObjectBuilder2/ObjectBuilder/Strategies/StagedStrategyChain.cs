using System;
using System.Collections.Generic;

namespace ObjectBuilder
{
    public class StagedStrategyChain<TStageEnum>
    {
        static readonly Array stageValues = Enum.GetValues(typeof(TStageEnum));
        readonly StagedStrategyChain<TStageEnum> innerChain;
        readonly object lockObject = new object();
        readonly Dictionary<TStageEnum, List<IBuilderStrategy>> stages;

        public StagedStrategyChain()
        {
            stages = new Dictionary<TStageEnum, List<IBuilderStrategy>>();

            foreach (TStageEnum stage in stageValues)
                stages[stage] = new List<IBuilderStrategy>();
        }

        public StagedStrategyChain(StagedStrategyChain<TStageEnum> innerChain)
            : this()
        {
            this.innerChain = innerChain;
        }

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

        protected IEnumerable<IBuilderStrategy> GetStrategiesInStage(TStageEnum stage)
        {
            if (innerChain != null)
                foreach (IBuilderStrategy strategy in innerChain.GetStrategiesInStage(stage))
                    yield return strategy;

            foreach (IBuilderStrategy strategy in stages[stage])
                yield return strategy;
        }

        public StrategyChain MakeStrategyChain()
        {
            lock (lockObject)
            {
                StrategyChain result = new StrategyChain();

                foreach (TStageEnum stage in stageValues)
                    result.AddRange(GetStrategiesInStage(stage));

                return result;
            }
        }
    }
}