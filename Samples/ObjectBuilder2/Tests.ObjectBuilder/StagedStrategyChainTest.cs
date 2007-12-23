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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void MultipleChildContainers()
        {
            StagedStrategyChain<FakeStage> innerChain = new StagedStrategyChain<FakeStage>();
            StagedStrategyChain<FakeStage> outerChain = new StagedStrategyChain<FakeStage>(innerChain);
            StagedStrategyChain<FakeStage> superChain = new StagedStrategyChain<FakeStage>(outerChain);

            FakeStrategy innerStrategy = new FakeStrategy();
            FakeStrategy outerStrategy = new FakeStrategy();
            FakeStrategy superStrategy = new FakeStrategy();
            innerChain.Add(innerStrategy, FakeStage.Stage1);
            outerChain.Add(outerStrategy, FakeStage.Stage1);
            superChain.Add(superStrategy, FakeStage.Stage1);

            StrategyChain chain = superChain.MakeStrategyChain();

            AssertOrder(chain, innerStrategy, outerStrategy, superStrategy);
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