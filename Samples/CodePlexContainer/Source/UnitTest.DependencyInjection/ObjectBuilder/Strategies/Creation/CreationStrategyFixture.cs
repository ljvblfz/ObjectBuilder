using System;
using System.Reflection;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class CreationStrategyFixture
    {
        [Test]
        public void CreationStrategyWithNoPoliciesFails()
        {
            MockBuilderContext ctx = CreateContext();

            Assert.Throws<ArgumentException>(
                delegate
                {
                    ctx.HeadOfChain.BuildUp(ctx, typeof(object), null, null);
                });
        }

        [Test]
        public void CreationStrategyUsesSingletonPolicyToLocateCreatedItems()
        {
            MockBuilderContext ctx = CreateContext();
            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            ctx.Policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));

            object obj = ctx.HeadOfChain.BuildUp(ctx, typeof(object), null, null);

            Assert.Equal(1, ctx.Lifetime.Count);
            Assert.Same(obj, ctx.Locator.Get(new DependencyResolutionLocatorKey(typeof(object), null)));
        }

        [Test]
        public void CreationStrategyOnlyLocatesItemIfSingletonPolicySetForThatType()
        {
            MockBuilderContext ctx = CreateContext();
            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            ctx.Policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));
            ctx.Policies.Set<ISingletonPolicy>(new SingletonPolicy(false), typeof(object), null);

            ctx.HeadOfChain.BuildUp(ctx, typeof(object), null, null);

            Assert.Equal(0, ctx.Lifetime.Count);
            Assert.Null(ctx.Locator.Get(new DependencyResolutionLocatorKey(typeof(object), null)));
        }

        [Test]
        public void AllCreatedDependenciesArePlacedIntoLocatorAndLifetimeContainer()
        {
            MockBuilderContext ctx = CreateContext();
            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            ctx.Policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));

            MockDependingObject obj = (MockDependingObject)ctx.HeadOfChain.BuildUp(ctx, typeof(MockDependingObject), null, null);

            Assert.Equal(2, ctx.Lifetime.Count);
            Assert.Same(obj, ctx.Locator.Get(new DependencyResolutionLocatorKey(typeof(MockDependingObject), null)));
            Assert.Same(obj.DependentObject, ctx.Locator.Get(new DependencyResolutionLocatorKey(typeof(MockDependentObject), null)));
        }

        [Test]
        public void InjectedDependencyIsReusedWhenDependingObjectIsCreatedTwice()
        {
            MockBuilderContext ctx = CreateContext();
            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            ctx.Policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));

            MockDependingObject obj1 = (MockDependingObject)ctx.HeadOfChain.BuildUp(ctx, typeof(MockDependingObject), null, null);
            MockDependingObject obj2 = (MockDependingObject)ctx.HeadOfChain.BuildUp(ctx, typeof(MockDependingObject), null, null);

            Assert.Same(obj1.DependentObject, obj2.DependentObject);
        }

        [Test]
        public void NamedObjectsOfSameTypeAreUnique()
        {
            MockBuilderContext ctx = CreateContext();
            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            ctx.Policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));

            object obj1 = ctx.HeadOfChain.BuildUp(ctx, typeof(object), null, "Foo");
            object obj2 = ctx.HeadOfChain.BuildUp(ctx, typeof(object), null, "Bar");

            Assert.Equal(2, ctx.Lifetime.Count);
            Assert.False(ReferenceEquals(obj1, obj2));
        }

        [Test]
        public void CreatingAbstractTypeThrows()
        {
            MockBuilderContext ctx = CreateContext();
            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            ctx.Policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));

            Assert.Throws<ArgumentException>(
                delegate
                {
                    ctx.HeadOfChain.BuildUp(ctx, typeof(AbstractClass), null, null);
                });
        }

        [Test]
        public void CanCreateValueTypes()
        {
            MockBuilderContext ctx = CreateContext();
            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());

            Assert.Equal(0, (int)ctx.HeadOfChain.BuildUp(ctx, typeof(int), null, null));
        }

        [Test]
        public void CannotCreateStrings()
        {
            MockBuilderContext ctx = CreateContext();
            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());

            Assert.Throws<ArgumentException>(
                delegate
                {
                    ctx.HeadOfChain.BuildUp(ctx, typeof(string), null, null);
                });
        }

        [Test]
        public void NotFindingAMatchingConstructorThrows()
        {
            MockBuilderContext ctx = CreateContext();
            FailingCreationPolicy policy = new FailingCreationPolicy();
            ctx.Policies.SetDefault<ICreationPolicy>(policy);

            Assert.Throws<ArgumentException>(
                delegate
                {
                    ctx.HeadOfChain.BuildUp(ctx, typeof(object), null, null);
                });
        }

        [Test]
        public void CreationStrategyWillLocateExistingObjects()
        {
            MockBuilderContext ctx = CreateContext();
            ctx.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            ctx.Policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));
            object obj = new object();

            ctx.HeadOfChain.BuildUp(ctx, typeof(object), obj, null);

            Assert.Equal(1, ctx.Lifetime.Count);
            Assert.Same(obj, ctx.Locator.Get(new DependencyResolutionLocatorKey(typeof(object), null)));
        }

        [Test]
        public void IncompatibleTypesThrows()
        {
            MockBuilderContext ctx = CreateContext();
            ConstructorInfo ci = typeof(MockObject).GetConstructor(new Type[] { typeof(int) });
            ICreationPolicy policy = new ConstructorPolicy(ci, new ValueParameter<string>(String.Empty));
            ctx.Policies.Set(policy, typeof(MockObject), null);

            Assert.Throws<ArgumentException>(
                delegate
                {
                    ctx.HeadOfChain.BuildUp(ctx, typeof(MockObject), null, null);
                });
        }

        // Helpers

        internal class MockObject
        {
            public int foo;

            public MockObject(int foo)
            {
                this.foo = foo;
            }
        }

        internal class FailingCreationPolicy : ICreationPolicy
        {
            public object[] GetParameters(IBuilderContext context,
                                          Type type,
                                          string id,
                                          ConstructorInfo ci)
            {
                return new object[] { };
            }

            public ConstructorInfo SelectConstructor(IBuilderContext context,
                                                     Type type,
                                                     string id)
            {
                return null;
            }
        }

        static MockBuilderContext CreateContext()
        {
            MockBuilderContext result = new MockBuilderContext();
            result.Strategies.Add(new SingletonStrategy());
            result.Strategies.Add(new CreationStrategy());
            return result;
        }

        abstract class AbstractClass {}

        class MockDependingObject
        {
            public object DependentObject;

            public MockDependingObject(MockDependentObject obj)
            {
                DependentObject = obj;
            }
        }

        class MockDependentObject {}
    }
}