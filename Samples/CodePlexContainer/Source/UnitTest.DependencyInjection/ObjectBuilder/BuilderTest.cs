using System;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class BuilderTest
    {
        [TestFixture]
        public class BuildUp
        {
            [Test]
            public void NullStrategies()
            {
                Builder builder = new Builder();

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         builder.BuildUp<object>(null, null, null, null, (string)null, null);
                                                     });
            }

            [Test]
            public void EmptyStrategies()
            {
                Builder builder = new Builder();
                StrategyChain strategies = new StrategyChain();

                Assert.Throws<ArgumentException>(delegate
                                                 {
                                                     builder.BuildUp<object>(null, null, null, strategies, null, null);
                                                 });
            }

            [Test]
            public void StrategyStagesRunInProperOrder()
            {
                Builder builder = new Builder();
                StrategyChain strategies = new StrategyChain();
                strategies.Add(new StringConcatStrategy("1"));
                strategies.Add(new StringConcatStrategy("2"));
                strategies.Add(new StringConcatStrategy("3"));
                strategies.Add(new StringConcatStrategy("4"));

                string s = builder.BuildUp<string>(null, null, null, strategies, null, null);

                Assert.Equal("1234", s);
            }

            [Test]
            public void PoliciesSetDuringBuildUpDoNotPersistAfterBuildUpComplete()
            {
                Builder builder = new Builder();
                PolicyList policies = new PolicyList();
                StrategyChain strategies = new StrategyChain();
                PolicySettingStrategy setStrategy = new PolicySettingStrategy();
                PolicyGettingStrategy getStrategy = new PolicyGettingStrategy();
                strategies.Add(setStrategy);
                strategies.Add(getStrategy);

                builder.BuildUp<object>(null, null, policies, strategies, null, null);

                Assert.Same(setStrategy.Policy, getStrategy.Policy);
                Assert.Null(policies.Get<FakePolicy>(typeof(object), null));
            }
        }

        [TestFixture]
        public class TearDown
        {
            [Test]
            public void NullItem()
            {
                Builder builder = new Builder();
                StrategyChain strategies = new StrategyChain();

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         builder.TearDown<object>(null, null, null, strategies, null);
                                                     });
            }

            [Test]
            public void NullStrategies()
            {
                Builder builder = new Builder();

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         builder.TearDown(null, null, null, null, new object());
                                                     });
            }

            [Test]
            public void EmptyStrategies()
            {
                Builder builder = new Builder();
                StrategyChain strategies = new StrategyChain();

                Assert.Throws<ArgumentException>(delegate
                                                 {
                                                     builder.TearDown(null, null, null, strategies, new object());
                                                 });
            }

            [Test]
            public void StrategiesRunInReverseOrder()
            {
                Builder builder = new Builder();
                StrategyChain strategies = new StrategyChain();
                strategies.Add(new StringConcatStrategy("1"));
                strategies.Add(new StringConcatStrategy("2"));
                strategies.Add(new StringConcatStrategy("3"));
                strategies.Add(new StringConcatStrategy("4"));

                string s = builder.TearDown(null, null, null, strategies, "");

                Assert.Equal("4321", s);
            }
        }

        // Helpers

        class FakePolicy : IBuilderPolicy {}

        class PolicySettingStrategy : BuilderStrategy
        {
            public FakePolicy Policy = new FakePolicy();

            public override object BuildUp(IBuilderContext context,
                                           Type typeToBuild,
                                           object existing,
                                           string idToBuild)
            {
                context.Policies.Set(Policy, typeof(object), null);
                return base.BuildUp(context, typeToBuild, existing, idToBuild);
            }
        }

        class PolicyGettingStrategy : BuilderStrategy
        {
            public FakePolicy Policy = null;

            public override object BuildUp(IBuilderContext context,
                                           Type typeToBuild,
                                           object existing,
                                           string idToBuild)
            {
                Policy = context.Policies.Get<FakePolicy>(typeof(object), null);
                return base.BuildUp(context, typeToBuild, existing, idToBuild);
            }
        }

        class StringConcatStrategy : BuilderStrategy
        {
            public string StringValue;

            public StringConcatStrategy(string value)
            {
                StringValue = value;
            }

            public override object BuildUp(IBuilderContext context,
                                           Type typeToBuild,
                                           object existing,
                                           string idToBuild)
            {
                return base.BuildUp(context, typeToBuild, AppendString(existing), idToBuild);
            }

            public override object TearDown(IBuilderContext context,
                                            object item)
            {
                return base.TearDown(context, AppendString(item));
            }

            string AppendString(object item)
            {
                string result;

                if (item == null)
                    result = StringValue;
                else
                    result = ((string)item) + StringValue;

                return result;
            }
        }
    }
}