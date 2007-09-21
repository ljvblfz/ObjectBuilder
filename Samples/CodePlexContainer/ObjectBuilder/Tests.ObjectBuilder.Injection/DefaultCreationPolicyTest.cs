using Xunit;

namespace ObjectBuilder
{
    public class DefaultCreationPolicyTest
    {
        [Test]
        public void ConstructorWithValueType()
        {
            MockBuilderContext ctx = CreateContext();

            CtorValueTypeObject result = (CtorValueTypeObject)ctx.HeadOfChain.BuildUp(ctx, typeof(CtorValueTypeObject), null);

            Assert.NotNull(result);
            Assert.Equal(0, result.IntValue);
        }

        static MockBuilderContext CreateContext()
        {
            MockBuilderContext result = new MockBuilderContext();
            result.Strategies.Add(new CreationStrategy());
            result.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            return result;
        }

        [Test]
        public void DependencyChainIsFollowed()
        {
            MockBuilderContext context = CreateContext();

            CascadingCtor result = (CascadingCtor)context.HeadOfChain.BuildUp(context, typeof(CascadingCtor), null);

            Assert.NotNull(result);
            Assert.NotNull(result.CtorObject);
            Assert.NotNull(result.CtorObject.Foo);
        }

        [Test]
        public void MultiParameterConstructor()
        {
            MockBuilderContext context = CreateContext();

            MultiParamCtor result = (MultiParamCtor)context.HeadOfChain.BuildUp(context, typeof(MultiParamCtor), null);

            Assert.NotNull(result);
            Assert.NotNull(result.O1);
            Assert.NotNull(result.O2);
        }

        [Test]
        public void ParameterizedConstructor()
        {
            MockBuilderContext context = CreateContext();

            ParameterizedCtor result = (ParameterizedCtor)context.HeadOfChain.BuildUp(context, typeof(ParameterizedCtor), null);

            Assert.NotNull(result);
            Assert.NotNull(result.Foo);
        }

        [Test]
        public void ParameterlessConstructor()
        {
            MockBuilderContext context = CreateContext();

            object result = context.HeadOfChain.BuildUp(context, typeof(object), null);

            Assert.NotNull(result);
        }

        [Test]
        public void ValueTypeCanBeConstructed()
        {
            MockBuilderContext context = CreateContext();

            int result = (int)context.HeadOfChain.BuildUp(context, typeof(int), null);

            Assert.Equal(0, result);
        }

        [Test]
        public void ValueTypeWithConstructor()
        {
            MockBuilderContext context = CreateContext();

            ValueTypeWithCtor result = (ValueTypeWithCtor)context.HeadOfChain.BuildUp(context, typeof(ValueTypeWithCtor), null);

            Assert.NotNull(result.ObjectValue);
        }

        class CascadingCtor
        {
            public readonly ParameterizedCtor CtorObject;

            public CascadingCtor(ParameterizedCtor ctorObject)
            {
                CtorObject = ctorObject;
            }
        }

        class CtorValueTypeObject
        {
            public readonly int IntValue;

            public CtorValueTypeObject(int i)
            {
                IntValue = i;
            }
        }

        class MultiParamCtor
        {
            public readonly object O1;
            public readonly object O2;

            public MultiParamCtor(object o1,
                                  object o2)
            {
                O1 = o1;
                O2 = o2;
            }
        }

        class ParameterizedCtor
        {
            public readonly object Foo;

            public ParameterizedCtor(object foo)
            {
                Foo = foo;
            }
        }

        internal struct ValueTypeWithCtor
        {
            public readonly object ObjectValue;

            public ValueTypeWithCtor(object o)
            {
                ObjectValue = o;
            }
        }
    }
}