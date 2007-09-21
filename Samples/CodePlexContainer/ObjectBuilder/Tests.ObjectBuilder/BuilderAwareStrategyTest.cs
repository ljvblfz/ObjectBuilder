using Xunit;

namespace ObjectBuilder
{
    public class BuilderAwareStrategyTest
    {
        [Test]
        public void BuildCallsClassWithInterface()
        {
            BuilderAwareStrategy strategy = new BuilderAwareStrategy();
            MockBuilderContext context = new MockBuilderContext();
            Aware obj = new Aware();

            context.Strategies.Add(strategy);

            context.HeadOfChain.BuildUp(context, typeof(Aware), obj);

            Assert.True(obj.OnBuiltUp__Called);
            Assert.False(obj.OnTearingDown__Called);
            Assert.Equal<object>(typeof(Aware), obj.OnBuiltUp_BuildKey);
        }

        [Test]
        public void BuildChecksConcreteTypeAndNotRequestedType()
        {
            BuilderAwareStrategy strategy = new BuilderAwareStrategy();
            MockBuilderContext context = new MockBuilderContext();
            Aware obj = new Aware();

            context.Strategies.Add(strategy);

            context.HeadOfChain.BuildUp(context, typeof(Ignorant), obj);

            Assert.True(obj.OnBuiltUp__Called);
            Assert.False(obj.OnTearingDown__Called);
        }

        [Test]
        public void BuildIgnoresClassWithoutInterface()
        {
            BuilderAwareStrategy strategy = new BuilderAwareStrategy();
            MockBuilderContext context = new MockBuilderContext();
            Ignorant obj = new Ignorant();

            context.Strategies.Add(strategy);

            context.HeadOfChain.BuildUp(context, typeof(Ignorant), obj);

            Assert.False(obj.OnBuiltUp__Called);
            Assert.False(obj.OnTearingDown__Called);
        }

        [Test]
        public void TearDownCallsClassWithInterface()
        {
            BuilderAwareStrategy strategy = new BuilderAwareStrategy();
            MockBuilderContext context = new MockBuilderContext();
            Aware obj = new Aware();

            context.Strategies.Add(strategy);

            context.HeadOfChain.TearDown(context, obj);

            Assert.False(obj.OnBuiltUp__Called);
            Assert.True(obj.OnTearingDown__Called);
        }

        [Test]
        public void TearDownIgnoresClassWithoutInterface()
        {
            BuilderAwareStrategy strategy = new BuilderAwareStrategy();
            MockBuilderContext context = new MockBuilderContext();
            Ignorant obj = new Ignorant();

            context.Strategies.Add(strategy);

            context.HeadOfChain.TearDown(context, obj);

            Assert.False(obj.OnBuiltUp__Called);
            Assert.False(obj.OnTearingDown__Called);
        }

        class Aware : Ignorant, IBuilderAware {}

        class Ignorant
        {
            public bool OnBuiltUp__Called = false;
            public object OnBuiltUp_BuildKey = null;
            public bool OnTearingDown__Called = false;

            public void OnBuiltUp(object buildKey)
            {
                OnBuiltUp__Called = true;
                OnBuiltUp_BuildKey = buildKey;
            }

            public void OnTearingDown()
            {
                OnTearingDown__Called = true;
            }
        }
    }
}