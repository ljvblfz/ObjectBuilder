using Xunit;

namespace ObjectBuilder
{
    public class BuildKeyMappingStrategyTest
    {
        [Fact]
        public void CanMakeArbitraryKeysToConcreteTypes()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(typeof(Foo)), "bar");
            BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            context.Strategies.Add(strategy);
            SpyStrategy spy = new SpyStrategy();
            context.Strategies.Add(spy);

            strategy.BuildUp(context, "bar", null);

            Assert.Equal<object>(typeof(Foo), spy.BuildKey);
        }

        [Fact]
        public void CanMapGenericsWithIdenticalGenericParameters()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(typeof(Foo<>)), typeof(IFoo<>));
            BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            context.Strategies.Add(strategy);
            SpyStrategy spy = new SpyStrategy();
            context.Strategies.Add(spy);

            strategy.BuildUp(context, typeof(IFoo<int>), null);

            Assert.Equal<object>(typeof(Foo<int>), spy.BuildKey);
        }

        [Fact]
        public void CanMapInterfacesToConcreteTypes()
        {
            MockBuilderContext context = new MockBuilderContext();
            context.Policies.Set<IBuildKeyMappingPolicy>(new BuildKeyMappingPolicy(typeof(Foo)), typeof(IFoo));
            BuildKeyMappingStrategy strategy = new BuildKeyMappingStrategy();
            context.Strategies.Add(strategy);
            SpyStrategy spy = new SpyStrategy();
            context.Strategies.Add(spy);

            strategy.BuildUp(context, typeof(IFoo), null);

            Assert.Equal<object>(typeof(Foo), spy.BuildKey);
        }

        class Foo : IFoo {}

        class Foo<T> : IFoo<T> {}

        interface IFoo {}

        interface IFoo<T> {}

        class SpyStrategy : BuilderStrategy
        {
            public object BuildKey;

            public override object BuildUp(IBuilderContext context,
                                           object buildKey,
                                           object existing)
            {
                BuildKey = buildKey;
                return null;
            }
        }
    }
}