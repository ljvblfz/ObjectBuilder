using System;
using NUnit.Framework;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class CreationParameterFixture
    {
        [Test]
        public void CreationParameterUsesStrategyChainToCreateObjects()
        {
            MockBuilderContext ctx = new MockBuilderContext();
            NullStrategy strategy = new NullStrategy();
            ctx.Strategies.Add(strategy);

            CreationParameter param = new CreationParameter(typeof(object));
            param.GetValue(ctx);

            Assert.IsTrue(strategy.WasCalled);
            Assert.AreEqual(typeof(object), strategy.TypeRequested);
        }

        [Test]
        public void CreationParameterCanCreateObjectsOfAGivenID()
        {
            MockBuilderContext ctx = new MockBuilderContext();
            NullStrategy strategy = new NullStrategy();
            ctx.Strategies.Add(strategy);

            CreationParameter param = new CreationParameter(typeof(object), "foo");
            param.GetValue(ctx);

            Assert.AreEqual("foo", strategy.IDRequested);
        }

        class NullStrategy : BuilderStrategy
        {
            public object IDRequested = null;
            public Type TypeRequested = null;
            public bool WasCalled = false;

            public override object BuildUp(IBuilderContext context,
                                           Type t,
                                           object existing,
                                           string id)
            {
                WasCalled = true;
                TypeRequested = t;
                IDRequested = id;

                return null;
            }
        }
    }
}