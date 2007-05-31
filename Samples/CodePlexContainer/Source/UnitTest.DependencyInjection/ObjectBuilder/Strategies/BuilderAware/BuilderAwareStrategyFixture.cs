using NUnit.Framework;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class BuilderAwareStrategyFixture
    {
        [Test]
        public void BuildIgnoresClassWithoutInterface()
        {
            BuilderAwareStrategy strategy = new BuilderAwareStrategy();
            MockBuilderContext context = new MockBuilderContext();
            IgnorantObject obj = new IgnorantObject();

            context.Strategies.Add(strategy);

            context.HeadOfChain.BuildUp(context, typeof(IgnorantObject), obj, null);

            Assert.IsFalse(obj.OnAssembledCalled);
            Assert.IsFalse(obj.OnDisassemblingCalled);
        }

        [Test]
        public void UnbuildIgnoresClassWithoutInterface()
        {
            BuilderAwareStrategy strategy = new BuilderAwareStrategy();
            MockBuilderContext context = new MockBuilderContext();
            IgnorantObject obj = new IgnorantObject();

            context.Strategies.Add(strategy);

            context.HeadOfChain.TearDown(context, obj);

            Assert.IsFalse(obj.OnAssembledCalled);
            Assert.IsFalse(obj.OnDisassemblingCalled);
        }

        [Test]
        public void BuildCallsClassWithInterface()
        {
            BuilderAwareStrategy strategy = new BuilderAwareStrategy();
            MockBuilderContext context = new MockBuilderContext();
            AwareObject obj = new AwareObject();

            context.Strategies.Add(strategy);

            context.HeadOfChain.BuildUp(context, typeof(AwareObject), obj, "foo");

            Assert.IsTrue(obj.OnAssembledCalled);
            Assert.IsFalse(obj.OnDisassemblingCalled);
            Assert.AreEqual("foo", obj.AssembledID);
        }

        [Test]
        public void UnbuildCallsClassWithInterface()
        {
            BuilderAwareStrategy strategy = new BuilderAwareStrategy();
            MockBuilderContext context = new MockBuilderContext();
            AwareObject obj = new AwareObject();

            context.Strategies.Add(strategy);

            context.HeadOfChain.TearDown(context, obj);

            Assert.IsFalse(obj.OnAssembledCalled);
            Assert.IsTrue(obj.OnDisassemblingCalled);
        }

        [Test]
        public void BuildChecksConcreteTypeAndNotRequestedType()
        {
            BuilderAwareStrategy strategy = new BuilderAwareStrategy();
            MockBuilderContext context = new MockBuilderContext();
            AwareObject obj = new AwareObject();

            context.Strategies.Add(strategy);

            context.HeadOfChain.BuildUp(context, typeof(IgnorantObject), obj, null);

            Assert.IsTrue(obj.OnAssembledCalled);
            Assert.IsFalse(obj.OnDisassemblingCalled);
        }

        class IgnorantObject
        {
            public bool OnAssembledCalled = false;
            public bool OnDisassemblingCalled = false;
            public string AssembledID = null;

            public void OnBuiltUp(string id)
            {
                OnAssembledCalled = true;
                AssembledID = id;
            }

            public void OnTearingDown()
            {
                OnDisassemblingCalled = true;
            }
        }

        class AwareObject : IgnorantObject, IBuilderAware {}
    }
}