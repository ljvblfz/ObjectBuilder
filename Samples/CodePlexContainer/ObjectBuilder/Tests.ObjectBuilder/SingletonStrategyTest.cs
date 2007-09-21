using System;
using Xunit;

namespace ObjectBuilder
{
    public class SingletonStrategyTest
    {
        static MockBuilderContext BuildContext()
        {
            MockBuilderContext ctx = new MockBuilderContext();

            ctx.Strategies.Add(new SingletonStrategy());
            ctx.Strategies.Add(new SimpleCreationStrategy());

            return ctx;
        }

        static MockBuilderContext BuildContext(IReadWriteLocator locator)
        {
            MockBuilderContext ctx = new MockBuilderContext(locator);

            ctx.Strategies.Add(new SingletonStrategy());
            ctx.Strategies.Add(new SimpleCreationStrategy());

            return ctx;
        }

        [Test]
        public void BuildingASingletonTwiceReturnsSameInstance()
        {
            MockBuilderContext ctx = BuildContext();
            ctx.Policies.Set<ISingletonPolicy>(new SingletonPolicy(true), typeof(object));

            object i1 = ctx.HeadOfChain.BuildUp(ctx, typeof(object), null);
            object i2 = ctx.HeadOfChain.BuildUp(ctx, typeof(object), null);

            Assert.Same(i1, i2);
        }

        [Test]
        public void ChildLocatorBeforeParent()
        {
            Locator parentLocator = new Locator();
            Locator childLocator = new Locator(parentLocator);
            MockBuilderContext ctx = BuildContext(childLocator);
            parentLocator.Add(typeof(string), "Hello world");
            childLocator.Add(typeof(string), "Goodbye world");

            string result = (string)ctx.HeadOfChain.BuildUp(ctx, typeof(string), null);

            Assert.Equal("Goodbye world", result);
        }

        [Test]
        public void SearchesParentLocator()
        {
            Locator parentLocator = new Locator();
            Locator childLocator = new Locator(parentLocator);
            MockBuilderContext ctx = BuildContext(childLocator);
            parentLocator.Add(typeof(string), "Hello world");

            string result = (string)ctx.HeadOfChain.BuildUp(ctx, typeof(string), null);

            Assert.Equal("Hello world", result);
        }

        class SimpleCreationStrategy : BuilderStrategy
        {
            public override object BuildUp(IBuilderContext context,
                                           object buildKey,
                                           object existing)
            {
                Type type;

                if (existing == null && TryGetTypeFromBuildKey(buildKey, out type))
                    existing = Activator.CreateInstance(type);

                return base.BuildUp(context, buildKey, existing);
            }
        }
    }
}