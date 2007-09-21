using System;
using Xunit;

namespace ObjectBuilder
{
    public class CreationStrategyTest
    {
        static MockBuilderContext CreateContext()
        {
            MockBuilderContext result = new MockBuilderContext();
            result.Strategies.Add(new SingletonStrategy());
            result.Strategies.Add(new CreationStrategy());
            return result;
        }

        [Test]
        public void DoesNotUsePolicyWhenPassedExistingObject()
        {
            object existing = new object();
            MockBuilderContext context = CreateContext();
            StubCreationPolicy policy = new StubCreationPolicy();
            context.Policies.SetDefault<ICreationPolicy>(policy);

            object result = context.HeadOfChain.BuildUp(context, typeof(object), existing);

            Assert.False(policy.Create__Called);
            Assert.Same(existing, result);
        }

        [Test]
        public void NoCreationPolicy()
        {
            MockBuilderContext context = CreateContext();

            Assert.Throws<InvalidOperationException>(
                delegate
                {
                    context.HeadOfChain.BuildUp(context, typeof(object), null);
                });
        }

        [Test]
        public void UsesPolicyToCreateObject()
        {
            object obj = new object();
            MockBuilderContext context = CreateContext();
            StubCreationPolicy policy = new StubCreationPolicy();
            policy.Create__Result = obj;
            context.Policies.SetDefault<ICreationPolicy>(policy);

            object result = context.HeadOfChain.BuildUp(context, typeof(object), null);

            Assert.True(policy.Create__Called);
            Assert.Same(context, policy.Create_Context);
            Assert.Same(typeof(object), policy.Create_BuildKey);
            Assert.Same(obj, result);
        }

        internal abstract class AbstractClass {}

        internal class Dependent {}

        internal class Depending
        {
            public readonly object ConstructorObject;

            public Depending(Dependent obj)
            {
                ConstructorObject = obj;
            }
        }
    }
}