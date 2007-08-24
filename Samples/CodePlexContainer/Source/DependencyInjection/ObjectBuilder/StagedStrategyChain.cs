using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class StagedStrategyChain<TStageEnum>
    {
        readonly StagedStrategyChain<TStageEnum> innerChain;
        readonly object lockObject = new object();
        readonly Dictionary<TStageEnum, List<IBuilderStrategy>> stages;
        static readonly Array stageValues = Enum.GetValues(typeof(TStageEnum));

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

        public StrategyChain MakeStrategyChain()
        {
            lock (lockObject)
            {
                StrategyChain result = new StrategyChain();

                foreach (TStageEnum stage in stageValues)
                {
                    if (innerChain != null)
                        result.AddRange(innerChain.stages[stage]);

                    result.AddRange(stages[stage]);
                }

                return result;
            }
        }
    }
}