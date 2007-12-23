using System;
using Xunit;

namespace ObjectBuilder
{
    public class DependencyResolverTest
    {
        [Fact]
        public void CanBuildObjectWhenNotPresent()
        {
            MockBuilderContext context = CreateContext();

            object result = DependencyResolver.Resolve(context, typeof(MockObject), NotPresentBehavior.Build);

            Assert.NotNull(result);
            Assert.IsType<MockObject>(result);
        }

        [Fact]
        public void CanReturnNullWhenNotPresent()
        {
            MockBuilderContext context = CreateContext();

            object result = DependencyResolver.Resolve(context, typeof(object), NotPresentBehavior.Null);

            Assert.Null(result);
        }

        [Fact]
        public void CanThrowWhenNotPresent()
        {
            MockBuilderContext context = CreateContext();

            Assert.Throws<DependencyMissingException>(
                delegate
                {
                    DependencyResolver.Resolve(context, typeof(object), NotPresentBehavior.Throw);
                });
        }

        static MockBuilderContext CreateContext()
        {
            MockBuilderContext result = new MockBuilderContext();
            result.Strategies.Add(new SingletonStrategy());
            result.Strategies.Add(new CreationStrategy());
            result.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            result.Policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));
            return result;
        }

        [Fact]
        public void NullBuildKey()
        {
            MockBuilderContext context = new MockBuilderContext();

            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    DependencyResolver.Resolve(context, null, NotPresentBehavior.Null);
                });
        }

        [Fact]
        public void NullContext()
        {
            Assert.Throws<ArgumentNullException>(
                delegate
                {
                    DependencyResolver.Resolve(null, "foo", NotPresentBehavior.Null);
                });
        }

        [Fact]
        public void ReturnsSingletonInstanceWhenPresent()
        {
            MockBuilderContext context = new MockBuilderContext();
            object obj = new object();
            context.Locator.Add("foo", obj);

            Assert.Same(obj, DependencyResolver.Resolve(context, "foo", NotPresentBehavior.Null));
        }

        interface IMockObject {}

        class MockObject : IMockObject {}
    }
}