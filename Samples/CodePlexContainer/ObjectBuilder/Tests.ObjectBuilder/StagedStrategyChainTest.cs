using System;
using Xunit;

namespace ObjectBuilder
{
    public class StagedStrategyChainTest
    {
        static void AssertOrder(IStrategyChain chain,
                                params FakeStrategy[] strategies)
        {
            IBuilderStrategy current = chain.Head;

            foreach (FakeStrategy strategy in strategies)
            {
                Assert.Same(strategy, current);
                current = chain.GetNext(current);
            }
        }

        [Test]
        public void InnerStrategiesComeBeforeOuterStrategiesInStrategyChain()
        {
            StagedStrategyChain<FakeStage> innerChain = new StagedStrategyChain<FakeStage>();
            StagedStrategyChain<FakeStage> outerChain = new StagedStrategyChain<FakeStage>(innerChain);
            FakeStrategy innerStrategy = new FakeStrategy();
            FakeStrategy outerStrategy = new FakeStrategy();
            innerChain.Add(innerStrategy, FakeStage.Stage1);
            outerChain.Add(outerStrategy, FakeStage.Stage1);

            StrategyChain chain = outerChain.MakeStrategyChain();

            AssertOrder(chain, innerStrategy, outerStrategy);
        }

        [Test]
        public void OrderingAcrossStagesForStrategyChain()
        {
            StagedStrategyChain<FakeStage> innerChain = new StagedStrategyChain<FakeStage>();
            StagedStrategyChain<FakeStage> outerChain = new StagedStrategyChain<FakeStage>(innerChain);
            FakeStrategy innerStage1 = new FakeStrategy();
            FakeStrategy innerStage2 = new FakeStrategy();
            FakeStrategy outerStage1 = new FakeStrategy();
            FakeStrategy outerStage2 = new FakeStrategy();
            innerChain.Add(innerStage1, FakeStage.Stage1);
            innerChain.Add(innerStage2, FakeStage.Stage2);
            outerChain.Add(outerStage1, FakeStage.Stage1);
            outerChain.Add(outerStage2, FakeStage.Stage2);

            StrategyChain chain = outerChain.MakeStrategyChain();

            AssertOrder(chain, innerStage1, outerStage1, innerStage2, outerStage2);
        }

        enum FakeStage
        {
            Stage1,
            Stage2,
        }

        class FakeStrategy : IBuilderStrategy
        {
            public object BuildUp(IBuilderContext context,
                                  object buildKey,
                                  object existing)
            {
                throw new NotImplementedException();
            }

            public object TearDown(IBuilderContext context,
                                   object item)
            {
                throw new NotImplementedException();
            }
        }
    }
}