using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class BuildKeyMappingStrategyTest
    {
        [Test]
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

        [Test]
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

        [Test]
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

        interface IFoo {}

        class Foo : IFoo {}

        interface IFoo<T> {}

        class Foo<T> : IFoo<T> {}
    }
}