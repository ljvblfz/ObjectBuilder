using System;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class StrategyListFixture
    {
        [Test]
        public void InnerStrategiesComeBeforeOuterStrategiesInStrategyChain()
        {
            StrategyList<FakeStage> innerList = new StrategyList<FakeStage>();
            StrategyList<FakeStage> outerList = new StrategyList<FakeStage>(innerList);
            FakeStrategy innerStrategy = new FakeStrategy();
            FakeStrategy outerStrategy = new FakeStrategy();
            innerList.Add(innerStrategy, FakeStage.Stage1);
            outerList.Add(outerStrategy, FakeStage.Stage1);

            IBuilderStrategyChain chain = outerList.MakeStrategyChain();

            AssertOrder(chain, innerStrategy, outerStrategy);
        }

        [Test]
        public void OuterStrategiesComeBeforeInnerStrategiesInReverseStrategyChain()
        {
            StrategyList<FakeStage> innerList = new StrategyList<FakeStage>();
            StrategyList<FakeStage> outerList = new StrategyList<FakeStage>(innerList);
            FakeStrategy innerStrategy = new FakeStrategy();
            FakeStrategy outerStrategy = new FakeStrategy();
            innerList.Add(innerStrategy, FakeStage.Stage1);
            outerList.Add(outerStrategy, FakeStage.Stage1);

            IBuilderStrategyChain chain = outerList.MakeReverseStrategyChain();

            AssertOrder(chain, outerStrategy, innerStrategy);
        }

        [Test]
        public void OrderingAcrossStagesForStrategyChain()
        {
            StrategyList<FakeStage> innerList = new StrategyList<FakeStage>();
            StrategyList<FakeStage> outerList = new StrategyList<FakeStage>(innerList);
            FakeStrategy innerStage1 = new FakeStrategy();
            FakeStrategy innerStage2 = new FakeStrategy();
            FakeStrategy outerStage1 = new FakeStrategy();
            FakeStrategy outerStage2 = new FakeStrategy();
            innerList.Add(innerStage1, FakeStage.Stage1);
            innerList.Add(innerStage2, FakeStage.Stage2);
            outerList.Add(outerStage1, FakeStage.Stage1);
            outerList.Add(outerStage2, FakeStage.Stage2);

            IBuilderStrategyChain chain = outerList.MakeStrategyChain();

            AssertOrder(chain, innerStage1, outerStage1, innerStage2, outerStage2);
        }

        [Test]
        public void OrderingAcrossStagesForReverseStrategyChain()
        {
            StrategyList<FakeStage> innerList = new StrategyList<FakeStage>();
            StrategyList<FakeStage> outerList = new StrategyList<FakeStage>(innerList);
            FakeStrategy innerStage1 = new FakeStrategy();
            FakeStrategy innerStage2 = new FakeStrategy();
            FakeStrategy outerStage1 = new FakeStrategy();
            FakeStrategy outerStage2 = new FakeStrategy();
            innerList.Add(innerStage1, FakeStage.Stage1);
            innerList.Add(innerStage2, FakeStage.Stage2);
            outerList.Add(outerStage1, FakeStage.Stage1);
            outerList.Add(outerStage2, FakeStage.Stage2);

            IBuilderStrategyChain chain = outerList.MakeReverseStrategyChain();

            AssertOrder(chain, outerStage2, innerStage2, outerStage1, innerStage1);
        }

        static void AssertOrder(IBuilderStrategyChain chain,
                                params FakeStrategy[] strategies)
        {
            IBuilderStrategy current = chain.Head;

            foreach (FakeStrategy strategy in strategies)
            {
                Assert.Same(strategy, current);
                current = chain.GetNext(current);
            }
        }

        enum FakeStage
        {
            Stage1,
            Stage2,
        }

        class FakeStrategy : IBuilderStrategy
        {
            public object BuildUp(IBuilderContext context,
                                  Type typeToBuild,
                                  object existing,
                                  string idToBuild)
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