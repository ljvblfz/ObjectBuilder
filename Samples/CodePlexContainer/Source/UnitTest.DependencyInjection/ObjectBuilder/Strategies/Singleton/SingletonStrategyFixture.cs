using NUnit.Framework;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class SingletonStrategyFixture
    {
        [Test]
        public void CreatingASingletonTwiceReturnsSameInstance()
        {
            MockBuilderContext ctx = BuildContext();
            ctx.Policies.Set<ISingletonPolicy>(new SingletonPolicy(true), typeof(Something), null);

            Something i1 = (Something)ctx.HeadOfChain.BuildUp(ctx, typeof(Something), null, null);
            Something i2 = (Something)ctx.HeadOfChain.BuildUp(ctx, typeof(Something), null, null);

            Assert.AreSame(i1, i2);
        }

        [Test]
        public void SingletonsCanBeBasedOnTypeAndID()
        {
            MockBuilderContext ctx = BuildContext();
            ctx.Policies.Set<ISingletonPolicy>(new SingletonPolicy(true), typeof(Something), "magickey");

            Something i1a = (Something)ctx.HeadOfChain.BuildUp(ctx, typeof(Something), null, "magickey");
            Something i1b = (Something)ctx.HeadOfChain.BuildUp(ctx, typeof(Something), null, "magickey");
            Something i2 = (Something)ctx.HeadOfChain.BuildUp(ctx, typeof(Something), null, null);
            Something i3 = (Something)ctx.HeadOfChain.BuildUp(ctx, typeof(Something), null, null);

            Assert.AreSame(i1a, i1b);
            Assert.IsTrue(i1a != i2);
            Assert.IsTrue(i2 != i3);
        }

        [Test]
        public void SearchesParentLocator()
        {
            Locator parentLocator = new Locator();
            Locator childLocator = new Locator(parentLocator);
            MockBuilderContext ctx = BuildContext(childLocator);
            parentLocator.Add(new DependencyResolutionLocatorKey(typeof(string), null), "Hello world");

            string result = (string)ctx.HeadOfChain.BuildUp(ctx, typeof(string), null, null);

            Assert.AreEqual("Hello world", result);
        }

        [Test]
        public void ChildLocatorBeforeParent()
        {
            Locator parentLocator = new Locator();
            Locator childLocator = new Locator(parentLocator);
            MockBuilderContext ctx = BuildContext(childLocator);
            parentLocator.Add(new DependencyResolutionLocatorKey(typeof(string), null), "Hello world");
            childLocator.Add(new DependencyResolutionLocatorKey(typeof(string), null), "Goodbye world");

            string result = (string)ctx.HeadOfChain.BuildUp(ctx, typeof(string), null, null);

            Assert.AreEqual("Goodbye world", result);
        }

        static MockBuilderContext BuildContext()
        {
            MockBuilderContext ctx = new MockBuilderContext();

            ctx.Strategies.Add(new SingletonStrategy());
            ctx.Strategies.Add(new CreationStrategy());

            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());

            return ctx;
        }

        static MockBuilderContext BuildContext(IReadWriteLocator locator)
        {
            MockBuilderContext ctx = new MockBuilderContext(locator);

            ctx.Strategies.Add(new SingletonStrategy());
            ctx.Strategies.Add(new CreationStrategy());

            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());

            return ctx;
        }

        class Something {}
    }
}