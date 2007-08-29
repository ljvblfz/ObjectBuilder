using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace ObjectBuilder
{
    [TestFixture]
    public class ConstructorReflectionStrategyTest
    {
        [Test]
        public void ZeroConstructorsOnReferenceType()
        {
            MockBuilderContext context = new MockBuilderContext();
            ConstructorReflectionStrategy strategy = new ConstructorReflectionStrategy();

            strategy.BuildUp(context, typeof(ZeroClass), null);

            ICreationPolicy policy = context.Policies.Get<ICreationPolicy>(typeof(ZeroClass));
            Assert.NotNull(policy);
            Assert.IsType<ZeroClass>(policy.Create(context, typeof(ZeroClass)));
        }

        [Test]
        public void ZeroConstructorsOnValueType()
        {
            MockBuilderContext context = new MockBuilderContext();
            ConstructorReflectionStrategy strategy = new ConstructorReflectionStrategy();

            strategy.BuildUp(context, typeof(ZeroStruct), null);

            Assert.Null(context.Policies.Get<ICreationPolicy>(typeof(ZeroStruct)));
        }

        [Test]
        public void NoDecoratedConstructors()
        {
            MockBuilderContext context = new MockBuilderContext();
            ConstructorReflectionStrategy strategy = new ConstructorReflectionStrategy();

            strategy.BuildUp(context, typeof(Undecorated), null);

            ICreationPolicy policy = context.Policies.Get<ICreationPolicy>(typeof(Undecorated));
            Undecorated undecorated = Assert.IsType<Undecorated>(policy.Create(context, typeof(Undecorated)));
            Assert.True(undecorated.Constructor__Called);
        }

        [Test]
        public void OneDecoratedConstructor()
        {
            MockBuilderContext context = new MockBuilderContext();
            ConstructorReflectionStrategy strategy = new ConstructorReflectionStrategy();

            strategy.BuildUp(context, typeof(Decorated), null);

            ICreationPolicy policy = context.Policies.Get<ICreationPolicy>(typeof(Decorated));
            Decorated decorated = Assert.IsType<Decorated>(policy.Create(context, typeof(Decorated)));
            Assert.True(decorated.Constructor__Called);
        }

        [Test]
        public void MultipleDecoratedConstructors()
        {
            MockBuilderContext context = new MockBuilderContext();
            ConstructorReflectionStrategy strategy = new ConstructorReflectionStrategy();

            Assert.Throws<InvalidAttributeException>(
                delegate
                {
                    strategy.BuildUp(context, typeof(MultiDecorated), null);
                });
        }

        internal class ZeroClass {}

        internal struct ZeroStruct {}

        internal class Undecorated
        {
            public bool Constructor__Called;

            public Undecorated()
            {
                Constructor__Called = true;
            }
        }

        internal class Decorated
        {
            public bool Constructor__Called;

            [InjectionConstructor]
            public Decorated()
            {
                Constructor__Called = true;
            }

#pragma warning disable 168
            public Decorated(int dummy)
            {
                Assert.True(false, "Incorrect constructor was called");
            }
#pragma warning restore 168
        }

        internal class MultiDecorated
        {
            [InjectionConstructor]
            public MultiDecorated() {}

#pragma warning disable 168
            [InjectionConstructor]
            public MultiDecorated(int dummy) {}
#pragma warning restore 168
        }
    }
}