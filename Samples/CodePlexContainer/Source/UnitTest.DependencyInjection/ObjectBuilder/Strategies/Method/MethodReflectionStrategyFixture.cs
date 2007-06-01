using System;
using NUnit.Framework;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [TestFixture]
    public class MethodReflectionStrategyFixture
    {
        // Invalid attribute combination

        [Test]
        [ExpectedException(typeof(InvalidAttributeException))]
        public void SpecifyingCreateNewAndDependencyThrows()
        {
            MockBuilderContext context = CreateContext();

            context.HeadOfChain.BuildUp(context, typeof(MockInvalidDualAttributes), null, null);
        }

        // Attribute Inheritance

        [Test]
        public void CanInheritDependencyAttribute()
        {
            MockBuilderContext context = CreateContext();

            MockDependingObjectDerived depending = (MockDependingObjectDerived)context.HeadOfChain.BuildUp(context, typeof(MockDependingObjectDerived), null, null);

            Assert.IsNotNull(depending);
            Assert.IsNotNull(depending.InjectedObject);
        }

        [Test]
        public void CanInheritCreateNewAttribute()
        {
            MockBuilderContext context = CreateContext();

            MockRequiresNewObjectDerived depending = (MockRequiresNewObjectDerived)context.HeadOfChain.BuildUp(context, typeof(MockRequiresNewObjectDerived), null, null);

            Assert.IsNotNull(depending);
            Assert.IsNotNull(depending.Foo);
        }

        // Non creatable stuff

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ThrowsIfConcreteTypeToCreateCannotBeCreated()
        {
            MockBuilderContext context = CreateContext();
            context.HeadOfChain.BuildUp(context, typeof(MockDependsOnInterface), null, null);
        }

        // Mode 1

        [Test]
        public void CreateNewAttributeAlwaysCreatesNewObject()
        {
            MockBuilderContext context;

            context = CreateContext();
            MockRequiresNewObject depending1 = (MockRequiresNewObject)context.HeadOfChain.BuildUp(context, typeof(MockRequiresNewObject), null, null);

            context = CreateContext();
            MockRequiresNewObject depending2 = (MockRequiresNewObject)context.HeadOfChain.BuildUp(context, typeof(MockRequiresNewObject), null, null);

            Assert.IsNotNull(depending1);
            Assert.IsNotNull(depending2);
            Assert.IsNotNull(depending1.Foo);
            Assert.IsNotNull(depending2.Foo);
            Assert.IsFalse(depending1.Foo == depending2.Foo);
        }

        [Test]
        public void NamedAndUnnamedObjectsInLocatorDontGetUsedForCreateNew()
        {
            MockBuilderContext context;
            object unnamed = new object();
            object named = new object();

            context = CreateContext();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), null), unnamed);
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), "Foo"), named);
            MockRequiresNewObject depending1 = (MockRequiresNewObject)context.HeadOfChain.BuildUp(context, typeof(MockRequiresNewObject), null, null);

            context = CreateContext();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), null), unnamed);
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), "Foo"), named);
            MockRequiresNewObject depending2 = (MockRequiresNewObject)context.HeadOfChain.BuildUp(context, typeof(MockRequiresNewObject), null, null);

            Assert.IsFalse(depending1.Foo == unnamed);
            Assert.IsFalse(depending1.Foo == unnamed);
            Assert.IsFalse(depending2.Foo == named);
            Assert.IsFalse(depending2.Foo == named);
        }

        // Mode 2

        [Test]
        public void CanInjectExistingUnnamedObjectIntoProperty()
        {
            // Mode 2, with an existing object
            MockBuilderContext context = CreateContext();
            object dependent = new object();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), null), dependent);

            object depending = context.HeadOfChain.BuildUp(context, typeof(MockDependingObject), null, null);

            Assert.IsNotNull(depending);
            Assert.IsTrue(depending is MockDependingObject);
            Assert.AreSame(dependent, ((MockDependingObject)depending).InjectedObject);
        }

        [Test]
        public void InjectionCreatingNewUnnamedObjectWillOnlyCreateOnce()
        {
            // Mode 2, both flavors
            MockBuilderContext context;

            context = CreateContext();
            MockDependingObject depending1 = (MockDependingObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingObject), null, null);

            context = CreateContext(context.Locator);
            MockDependingObject depending2 = (MockDependingObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingObject), null, null);

            Assert.AreSame(depending1.InjectedObject, depending2.InjectedObject);
        }

        [Test]
        public void InjectionCreatesNewObjectIfNotExisting()
        {
            // Mode 2, no existing object
            MockBuilderContext context = CreateContext();

            object depending = context.HeadOfChain.BuildUp(context, typeof(MockDependingObject), null, null);

            Assert.IsNotNull(depending);
            Assert.IsTrue(depending is MockDependingObject);
            Assert.IsNotNull(((MockDependingObject)depending).InjectedObject);
        }

        [Test]
        public void CanInjectNewInstanceWithExplicitTypeIfNotExisting()
        {
            // Mode 2, explicit type
            MockBuilderContext context = CreateContext();

            MockDependsOnIFoo depending = (MockDependsOnIFoo)context.HeadOfChain.BuildUp(
                                                                 context, typeof(MockDependsOnIFoo), null, null);

            Assert.IsNotNull(depending);
            Assert.IsNotNull(depending.Foo);
        }

        // Mode 3

        [Test]
        public void CanInjectExistingNamedObjectIntoProperty()
        {
            // Mode 3, with an existing object
            MockBuilderContext context = CreateContext();
            object dependent = new object();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), "Foo"), dependent);

            object depending = context.HeadOfChain.BuildUp(context, typeof(MockDependingNamedObject), null, null);

            Assert.IsNotNull(depending);
            Assert.IsTrue(depending is MockDependingNamedObject);
            Assert.AreSame(dependent, ((MockDependingNamedObject)depending).InjectedObject);
        }

        [Test]
        public void InjectionCreatingNewNamedObjectWillOnlyCreateOnce()
        {
            // Mode 3, both flavors
            MockBuilderContext context;

            context = CreateContext();
            MockDependingNamedObject depending1 = (MockDependingNamedObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingNamedObject), null, null);

            context = CreateContext(context.Locator);
            MockDependingNamedObject depending2 = (MockDependingNamedObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingNamedObject), null, null);

            Assert.AreSame(depending1.InjectedObject, depending2.InjectedObject);
        }

        [Test]
        public void InjectionCreatesNewNamedObjectIfNotExisting()
        {
            // Mode 3, no existing object
            MockBuilderContext context = CreateContext();

            MockDependingNamedObject depending = (MockDependingNamedObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingNamedObject), null, null);

            Assert.IsNotNull(depending);
            Assert.IsNotNull(depending.InjectedObject);
        }

        [Test]
        public void CanInjectNewNamedInstanceWithExplicitTypeIfNotExisting()
        {
            // Mode 3, explicit type
            MockBuilderContext context = CreateContext();

            MockDependsOnNamedIFoo depending = (MockDependsOnNamedIFoo)context.HeadOfChain.BuildUp(context, typeof(MockDependsOnNamedIFoo), null, null);

            Assert.IsNotNull(depending);
            Assert.IsNotNull(depending.Foo);
        }

        // Mode 2 & 3 together

        [Test]
        public void NamedAndUnnamedObjectsDontCollide()
        {
            MockBuilderContext context = CreateContext();
            object dependent = new object();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), null), dependent);

            MockDependingNamedObject depending = (MockDependingNamedObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingNamedObject), null, null);

            Assert.IsFalse(ReferenceEquals(dependent, depending.InjectedObject));
        }

        // Mode 4

        [Test]
        public void PropertyIsNullIfUnnamedNotExists()
        {
            // Mode 4, no object provided
            MockBuilderContext context = CreateContext();

            MockOptionalDependingObject depending = (MockOptionalDependingObject)context.HeadOfChain.BuildUp(context, typeof(MockOptionalDependingObject), null, null);

            Assert.IsNotNull(depending);
            Assert.IsNull(depending.InjectedObject);
        }

        [Test]
        public void CanInjectExistingUnnamedObjectIntoOptionalDependentProperty()
        {
            // Mode 4, with an existing object
            MockBuilderContext context = CreateContext();
            object dependent = new object();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), null), dependent);

            object depending = context.HeadOfChain.BuildUp(context, typeof(MockOptionalDependingObject), null, null);

            Assert.IsNotNull(depending);
            Assert.IsTrue(depending is MockOptionalDependingObject);
            Assert.AreSame(dependent, ((MockOptionalDependingObject)depending).InjectedObject);
        }

        // Mode 5

        [Test]
        public void PropertyIsNullIfNamedNotExists()
        {
            // Mode 5, no object provided
            MockBuilderContext context = CreateContext();

            MockOptionalDependingObjectWithName depending = (MockOptionalDependingObjectWithName)context.HeadOfChain.BuildUp(context, typeof(MockOptionalDependingObjectWithName), null, null);

            Assert.IsNotNull(depending);
            Assert.IsNull(depending.InjectedObject);
        }

        [Test]
        public void CanInjectExistingNamedObjectIntoOptionalDependentProperty()
        {
            // Mode 5, with an existing object
            MockBuilderContext context = CreateContext();
            object dependent = new object();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), "Foo"), dependent);

            object depending = context.HeadOfChain.BuildUp(context, typeof(MockOptionalDependingObjectWithName), null, null);

            Assert.IsNotNull(depending);
            Assert.IsTrue(depending is MockOptionalDependingObjectWithName);
            Assert.AreSame(dependent, ((MockOptionalDependingObjectWithName)depending).InjectedObject);
        }

        // NotPresentBehavior.Throw Tests

        [Test]
        [ExpectedException(typeof(DependencyMissingException))]
        public void StrategyThrowsIfObjectNotPresent()
        {
            MockBuilderContext context = CreateContext();

            context.HeadOfChain.BuildUp(context, typeof(ThrowingMockObject), null, null);
        }

        [Test]
        [ExpectedException(typeof(DependencyMissingException))]
        public void StrategyThrowsIfNamedObjectNotPresent()
        {
            MockBuilderContext context = CreateContext();

            context.HeadOfChain.BuildUp(context, typeof(NamedThrowingMockObject), null, null);
        }

        // Helpers

        MockBuilderContext CreateContext()
        {
            return CreateContext(new Locator());
        }

        MockBuilderContext CreateContext(IReadWriteLocator locator)
        {
            MockBuilderContext result = new MockBuilderContext(locator);
            result.Strategies.Add(new SingletonStrategy());
            result.Strategies.Add(new MethodReflectionStrategy());
            result.Strategies.Add(new CreationStrategy());
            result.Strategies.Add(new MethodExecutionStrategy());
            result.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            result.Policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));
            return result;
        }

        public class ThrowingMockObject
        {
            [InjectionMethod]
            public void SetValue([Dependency(NotPresentBehavior = NotPresentBehavior.Throw)] object obj) {}
        }

        public class NamedThrowingMockObject
        {
            [InjectionMethod]
            public void SetValue([Dependency(Name = "Foo", NotPresentBehavior = NotPresentBehavior.Throw)] object obj) {}
        }

        public class MockInvalidDualAttributes
        {
            [InjectionMethod]
            public void SetValue([CreateNew] [Dependency] int dummy) {}
        }

        public class MockDependsOnInterface
        {
            [InjectionMethod]
            public void DoSomething(IFoo foo) {}
        }

        public class MockDependingObject
        {
            public object InjectedObject;

            [InjectionMethod]
            public virtual void DoSomething([Dependency] object injectedObject)
            {
                InjectedObject = injectedObject;
            }
        }

        public class MockDependingObjectDerived : MockDependingObject
        {
            public override void DoSomething(object injectedObject)
            {
                base.DoSomething(injectedObject);
            }
        }

        public class MockOptionalDependingObject
        {
            public object InjectedObject;

            [InjectionMethod]
            public void SetObject([Dependency(NotPresentBehavior = NotPresentBehavior.ReturnNull)] object foo)
            {
                InjectedObject = foo;
            }
        }

        public class MockOptionalDependingObjectWithName
        {
            public object InjectedObject;

            [InjectionMethod]
            public void SetObject([Dependency(Name = "Foo", NotPresentBehavior = NotPresentBehavior.ReturnNull)] object foo)
            {
                InjectedObject = foo;
            }
        }

        public class MockDependingNamedObject
        {
            public object InjectedObject;

            [InjectionMethod]
            public void SetObject([Dependency(Name = "Foo")] object foo)
            {
                InjectedObject = foo;
            }
        }

        public class MockDependsOnIFoo
        {
            public IFoo Foo;

            [InjectionMethod]
            public void SetFoo([Dependency(CreateType = typeof(Foo))] IFoo foo)
            {
                Foo = foo;
            }
        }

        public class MockDependsOnNamedIFoo
        {
            public IFoo Foo;

            [InjectionMethod]
            public void SetFoo([Dependency(Name = "Foo", CreateType = typeof(Foo))] IFoo foo)
            {
                Foo = foo;
            }
        }

        public class MockRequiresNewObject
        {
            public object Foo;

            [InjectionMethod]
            public virtual void SetFoo([CreateNew] object foo)
            {
                Foo = foo;
            }
        }

        public class MockRequiresNewObjectDerived : MockRequiresNewObject
        {
            public override void SetFoo(object foo)
            {
                base.SetFoo(foo);
            }
        }

        public interface IFoo {}

        public class Foo : IFoo {}
    }
}