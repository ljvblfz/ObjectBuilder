using System;
using Xunit;

namespace ObjectBuilder
{
    public class BuilderTest
    {
        public class BuildUp
        {
            [Fact]
            public void EmptyStrategies()
            {
                Builder builder = new Builder();
                StrategyChain strategies = new StrategyChain();

                Assert.Throws<ArgumentNullException>(
                    delegate
                    {
                        builder.BuildUp<object>(null, null, null, strategies, null, null);
                    });
            }

            [Fact]
            public void NullStrategies()
            {
                Builder builder = new Builder();

                Assert.Throws<ArgumentNullException>(
                    delegate
                    {
                        builder.BuildUp<object>(null, null, null, null, null, null);
                    });
            }

            [Fact]
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
                Assert.Null(policies.Get<FakePolicy>(typeof(object)));
            }

            [Fact]
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
        }

        public class TearDown
        {
            [Fact]
            public void EmptyStrategies()
            {
                Builder builder = new Builder();
                StrategyChain strategies = new StrategyChain();

                Assert.Throws<ArgumentNullException>(
                    delegate
                    {
                        builder.TearDown(null, null, null, strategies, new object());
                    });
            }

            [Fact]
            public void NullItem()
            {
                Builder builder = new Builder();
                StrategyChain strategies = new StrategyChain();

                Assert.Throws<ArgumentNullException>(
                    delegate
                    {
                        builder.TearDown<object>(null, null, null, strategies, null);
                    });
            }

            [Fact]
            public void NullStrategies()
            {
                Builder builder = new Builder();

                Assert.Throws<ArgumentNullException>(
                    delegate
                    {
                        builder.TearDown(null, null, null, null, new object());
                    });
            }

            [Fact]
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

        class PolicyGettingStrategy : BuilderStrategy
        {
            public FakePolicy Policy = null;

            public override object BuildUp(IBuilderContext context,
                                           object buildKey,
                                           object existing)
            {
                Policy = context.Policies.Get<FakePolicy>(typeof(object));
                return base.BuildUp(context, buildKey, existing);
            }
        }

        class PolicySettingStrategy : BuilderStrategy
        {
            public readonly FakePolicy Policy = new FakePolicy();

            public override object BuildUp(IBuilderContext context,
                                           object buildKey,
                                           object existing)
            {
                context.Policies.Set(Policy, typeof(object));
                return base.BuildUp(context, buildKey, existing);
            }
        }

        class StringConcatStrategy : BuilderStrategy
        {
            public readonly string StringValue;

            public StringConcatStrategy(string value)
            {
                StringValue = value;
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

            public override object BuildUp(IBuilderContext context,
                                           object buildKey,
                                           object existing)
            {
                return base.BuildUp(context, buildKey, AppendString(existing));
            }

            public override object TearDown(IBuilderContext context,
                                            object item)
            {
                return base.TearDown(context, AppendString(item));
            }
        }
    }
}