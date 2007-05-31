using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    // These "modes" describe the classed of behavior provided by DI.
    // 1. I need a new X. Don't reuse any existing ones.
    // 2. I need the unnamed X. Create it if it doesn't exist, else return the existing one.
    // 3. I need the X named Y. Create it if it doesn't exist, else return the existing one.
    // 4. I want the unnamed X. Return null if it doesn't exist.
    // 5. I want the X named Y. Return null if it doesn't exist.

    [TestFixture]
    public class ConstructorReflectionStrategyFixture
    {
        // Value type creation

        [Test]
        public void CanCreateValueTypesWithConstructorInjectionStrategyInPlace()
        {
            MockBuilderContext context = CreateContext();

            Assert.Equal(0, (int)context.HeadOfChain.BuildUp(context, typeof(int), null, null));
        }

        // Invalid attribute combination

        [Test]
        [ExpectedException(typeof(InvalidAttributeException))]
        public void SpecifyingMultipleConstructorsThrows()
        {
            MockBuilderContext context = CreateContext();

            context.HeadOfChain.BuildUp(context, typeof(MockInvalidDualConstructorAttributes), null, null);
        }

        [Test]
        [ExpectedException(typeof(InvalidAttributeException))]
        public void SpecifyingCreateNewAndDependencyThrows()
        {
            MockBuilderContext context = CreateContext();

            context.HeadOfChain.BuildUp(context, typeof(MockInvalidDualParameterAttributes), null, null);
        }

        // Default behavior

        [Test]
        public void DefaultBehaviorIsMode2ForUndecoratedParameter()
        {
            MockBuilderContext context = CreateContext();

            MockUndecoratedObject obj1 = (MockUndecoratedObject)context.HeadOfChain.BuildUp(context, typeof(MockUndecoratedObject), null, null);
            MockUndecoratedObject obj2 = (MockUndecoratedObject)context.HeadOfChain.BuildUp(context, typeof(MockUndecoratedObject), null, null);

            Assert.Same(obj1.Foo, obj2.Foo);
        }

        [Test]
        public void WhenSingleConstructorIsPresentDecorationIsntRequired()
        {
            MockBuilderContext context = CreateContext();

            MockUndecoratedConstructorObject obj1 = (MockUndecoratedConstructorObject)context.HeadOfChain.BuildUp(context, typeof(MockUndecoratedConstructorObject), null, null);
            MockUndecoratedConstructorObject obj2 = (MockUndecoratedConstructorObject)context.HeadOfChain.BuildUp(context, typeof(MockUndecoratedConstructorObject), null, null);

            Assert.NotNull(obj1.Foo);
            Assert.Same(obj1.Foo, obj2.Foo);
        }

        // Mode 1

        [Test]
        public void CreateNewAttributeAlwaysCreatesNewObject()
        {
            MockBuilderContext context = CreateContext();

            MockRequiresNewObject depending1 = (MockRequiresNewObject)context.HeadOfChain.BuildUp(context, typeof(MockRequiresNewObject), null, "Foo");
            MockRequiresNewObject depending2 = (MockRequiresNewObject)context.HeadOfChain.BuildUp(context, typeof(MockRequiresNewObject), null, "Bar");

            Assert.NotNull(depending1);
            Assert.NotNull(depending2);
            Assert.NotNull(depending1.Foo);
            Assert.NotNull(depending2.Foo);
            Assert.False(ReferenceEquals(depending1.Foo, depending2.Foo));
        }

        [Test]
        public void NamedAndUnnamedObjectsInLocatorDontGetUsedForCreateNew()
        {
            MockBuilderContext context = CreateContext();
            object unnamed = new object();
            object named = new object();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), null), unnamed);
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), "Foo"), named);

            MockRequiresNewObject depending1 = (MockRequiresNewObject)context.HeadOfChain.BuildUp(context, typeof(MockRequiresNewObject), null, null);
            MockRequiresNewObject depending2 = (MockRequiresNewObject)context.HeadOfChain.BuildUp(context, typeof(MockRequiresNewObject), null, null);

            Assert.False(depending1.Foo == unnamed);
            Assert.False(depending2.Foo == unnamed);
            Assert.False(depending1.Foo == named);
            Assert.False(depending2.Foo == named);
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

            Assert.NotNull(depending);
            Assert.True(depending is MockDependingObject);
            Assert.Same(dependent, ((MockDependingObject)depending).InjectedObject);
        }

        [Test]
        public void InjectionCreatingNewUnnamedObjectWillOnlyCreateOnce()
        {
            // Mode 2, both flavors
            MockBuilderContext context = CreateContext();

            MockDependingObject depending1 = (MockDependingObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingObject), null, null);
            MockDependingObject depending2 = (MockDependingObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingObject), null, null);

            Assert.Same(depending1.InjectedObject, depending2.InjectedObject);
        }

        [Test]
        public void InjectionCreatesNewObjectIfNotExisting()
        {
            // Mode 2, no existing object
            MockBuilderContext context = CreateContext();

            object depending = context.HeadOfChain.BuildUp(context, typeof(MockDependingObject), null, null);

            Assert.NotNull(depending);
            Assert.True(depending is MockDependingObject);
            Assert.NotNull(((MockDependingObject)depending).InjectedObject);
        }

        [Test]
        public void CanInjectNewInstanceWithExplicitTypeIfNotExisting()
        {
            // Mode 2, explicit type
            MockBuilderContext context = CreateContext();

            MockDependsOnIFoo depending = (MockDependsOnIFoo)context.HeadOfChain.BuildUp(
                                                                 context, typeof(MockDependsOnIFoo), null, null);

            Assert.NotNull(depending);
            Assert.NotNull(depending.Foo);
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

            Assert.NotNull(depending);
            Assert.True(depending is MockDependingNamedObject);
            Assert.Same(dependent, ((MockDependingNamedObject)depending).InjectedObject);
        }

        [Test]
        public void InjectionCreatingNewNamedObjectWillOnlyCreateOnce()
        {
            // Mode 3, both flavors
            MockBuilderContext context = CreateContext();

            MockDependingNamedObject depending1 = (MockDependingNamedObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingNamedObject), null, null);
            MockDependingNamedObject depending2 = (MockDependingNamedObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingNamedObject), null, null);

            Assert.Same(depending1.InjectedObject, depending2.InjectedObject);
        }

        [Test]
        public void InjectionCreatesNewNamedObjectIfNotExisting()
        {
            // Mode 3, no existing object
            MockBuilderContext context = CreateContext();

            MockDependingNamedObject depending = (MockDependingNamedObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingNamedObject), null, null);

            Assert.NotNull(depending);
            Assert.NotNull(depending.InjectedObject);
        }

        [Test]
        public void CanInjectNewNamedInstanceWithExplicitTypeIfNotExisting()
        {
            // Mode 3, explicit type
            MockBuilderContext context = CreateContext();

            MockDependsOnNamedIFoo depending = (MockDependsOnNamedIFoo)context.HeadOfChain.BuildUp(context, typeof(MockDependsOnNamedIFoo), null, null);

            Assert.NotNull(depending);
            Assert.NotNull(depending.Foo);
        }

        // Mode 2 & 3 together

        [Test]
        public void NamedAndUnnamedObjectsDontCollide()
        {
            MockBuilderContext context = CreateContext();
            object dependent = new object();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), null), dependent);

            MockDependingNamedObject depending = (MockDependingNamedObject)context.HeadOfChain.BuildUp(context, typeof(MockDependingNamedObject), null, null);

            Assert.False(ReferenceEquals(dependent, depending.InjectedObject));
        }

        // Mode 4

        [Test]
        public void PropertyIsNullIfUnnamedNotExists()
        {
            // Mode 4, no object provided
            MockBuilderContext context = CreateContext();

            MockOptionalDependingObject depending = (MockOptionalDependingObject)context.HeadOfChain.BuildUp(context, typeof(MockOptionalDependingObject), null, null);

            Assert.NotNull(depending);
            Assert.Null(depending.InjectedObject);
        }

        [Test]
        public void CanInjectExistingUnnamedObjectIntoOptionalDependentProperty()
        {
            // Mode 4, with an existing object
            MockBuilderContext context = CreateContext();
            object dependent = new object();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), null), dependent);

            object depending = context.HeadOfChain.BuildUp(context, typeof(MockOptionalDependingObject), null, null);

            Assert.NotNull(depending);
            Assert.True(depending is MockOptionalDependingObject);
            Assert.Same(dependent, ((MockOptionalDependingObject)depending).InjectedObject);
        }

        // Mode 5

        [Test]
        public void PropertyIsNullIfNamedNotExists()
        {
            // Mode 5, no object provided
            MockBuilderContext context = CreateContext();

            MockOptionalDependingObjectWithName depending = (MockOptionalDependingObjectWithName)context.HeadOfChain.BuildUp(context, typeof(MockOptionalDependingObjectWithName), null, null);

            Assert.NotNull(depending);
            Assert.Null(depending.InjectedObject);
        }

        [Test]
        public void CanInjectExistingNamedObjectIntoOptionalDependentProperty()
        {
            // Mode 5, with an existing object
            MockBuilderContext context = CreateContext();
            object dependent = new object();
            context.Locator.Add(new DependencyResolutionLocatorKey(typeof(object), "Foo"), dependent);

            object depending = context.HeadOfChain.BuildUp(context, typeof(MockOptionalDependingObjectWithName), null, null);

            Assert.NotNull(depending);
            Assert.True(depending is MockOptionalDependingObjectWithName);
            Assert.Same(dependent, ((MockOptionalDependingObjectWithName)depending).InjectedObject);
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

        // SearchMode Tests

        [Test]
        public void CanSearchDependencyUp()
        {
            Locator parent = new Locator();
            parent.Add(new DependencyResolutionLocatorKey(typeof(int), null), 25);
            Locator child = new Locator(parent);
            MockBuilderContext context = CreateContext(child);

            context.HeadOfChain.BuildUp(context, typeof(SearchUpMockObject), null, null);
        }

        [Test]
        [ExpectedException(typeof(DependencyMissingException))]
        public void LocalSearchFailsIfDependencyIsOnlyUpstream()
        {
            Locator parent = new Locator();
            parent.Add(new DependencyResolutionLocatorKey(typeof(int), null), 25);
            Locator child = new Locator(parent);
            MockBuilderContext context = CreateContext(child);

            context.HeadOfChain.BuildUp(context, typeof(SearchLocalMockObject), null, null);
        }

        [Test]
        public void LocalSearchGetsLocalIfDependencyIsAlsoUpstream()
        {
            Locator parent = new Locator();
            parent.Add(new DependencyResolutionLocatorKey(typeof(int), null), 25);
            Locator child = new Locator(parent);
            child.Add(new DependencyResolutionLocatorKey(typeof(int), null), 15);
            MockBuilderContext context = CreateContext(child);

            SearchLocalMockObject obj = (SearchLocalMockObject)context.HeadOfChain.BuildUp(context, typeof(SearchLocalMockObject), null, null);

            Assert.Equal(15, obj.Value);
        }

        // Helpers

        MockBuilderContext CreateContext()
        {
            return CreateContext(new Locator());
        }

        MockBuilderContext CreateContext(Locator locator)
        {
            MockBuilderContext result = new MockBuilderContext(locator);
            result.Strategies.Add(new SingletonStrategy());
            result.Strategies.Add(new ConstructorReflectionStrategy());
            result.Strategies.Add(new CreationStrategy());
            result.Policies.SetDefault<ICreationPolicy>(new DefaultCreationPolicy());
            result.Policies.SetDefault<ISingletonPolicy>(new SingletonPolicy(true));
            return result;
        }

        public class SearchUpMockObject
        {
            public int Value;

            public SearchUpMockObject(
                [Dependency(SearchMode = SearchMode.Up, NotPresentBehavior = NotPresentBehavior.Throw)] int value)
            {
                Value = value;
            }
        }

        public class SearchLocalMockObject
        {
            public int Value;

            public SearchLocalMockObject(
                [Dependency(SearchMode = SearchMode.Local, NotPresentBehavior = NotPresentBehavior.Throw)] int value
                )
            {
                Value = value;
            }
        }

        public class ThrowingMockObject
        {
            [InjectionConstructor]
            public ThrowingMockObject([Dependency(NotPresentBehavior = NotPresentBehavior.Throw)] object foo) {}
        }

        public class NamedThrowingMockObject
        {
            [InjectionConstructor]
            public NamedThrowingMockObject([Dependency(Name = "Foo", NotPresentBehavior = NotPresentBehavior.Throw)] object foo) {}
        }

        public class MockDependingObject
        {
            object injectedObject;

            public MockDependingObject([Dependency] object injectedObject)
            {
                this.injectedObject = injectedObject;
            }

            public virtual object InjectedObject
            {
                get { return injectedObject; }
                set { injectedObject = value; }
            }
        }

        public class MockOptionalDependingObject
        {
            object injectedObject;

            public MockOptionalDependingObject
                (
                [Dependency(NotPresentBehavior = NotPresentBehavior.ReturnNull)] object injectedObject
                )
            {
                this.injectedObject = injectedObject;
            }

            public object InjectedObject
            {
                get { return injectedObject; }
                set { injectedObject = value; }
            }
        }

        public class MockOptionalDependingObjectWithName
        {
            object injectedObject;

            public MockOptionalDependingObjectWithName
                (
                [Dependency(Name = "Foo", NotPresentBehavior = NotPresentBehavior.ReturnNull)] object injectedObject
                )
            {
                this.injectedObject = injectedObject;
            }

            public object InjectedObject
            {
                get { return injectedObject; }
                set { injectedObject = value; }
            }
        }

        public class MockDependingNamedObject
        {
            object injectedObject;

            public MockDependingNamedObject([Dependency(Name = "Foo")] object injectedObject)
            {
                this.injectedObject = injectedObject;
            }

            public object InjectedObject
            {
                get { return injectedObject; }
                set { injectedObject = value; }
            }
        }

        public class MockDependsOnIFoo
        {
            IFoo foo;

            public MockDependsOnIFoo([Dependency(CreateType = typeof(Foo))] IFoo foo)
            {
                this.foo = foo;
            }

            public IFoo Foo
            {
                get { return foo; }
                set { foo = value; }
            }
        }

        public class MockDependsOnNamedIFoo
        {
            IFoo foo;

            public MockDependsOnNamedIFoo([Dependency(Name = "Foo", CreateType = typeof(Foo))] IFoo foo)
            {
                this.foo = foo;
            }

            public IFoo Foo
            {
                get { return foo; }
                set { foo = value; }
            }
        }

        public class MockRequiresNewObject
        {
            object foo;

            public MockRequiresNewObject([CreateNew] object foo)
            {
                this.foo = foo;
            }

            public virtual object Foo
            {
                get { return foo; }
                set { foo = value; }
            }
        }

        public interface IFoo {}

        public class Foo : IFoo {}

        class MockInvalidDualParameterAttributes
        {
            [InjectionConstructor]
            public MockInvalidDualParameterAttributes([CreateNew] [Dependency] object obj) {}
        }

        class MockInvalidDualConstructorAttributes
        {
            [InjectionConstructor]
            public MockInvalidDualConstructorAttributes(object obj) {}

            [InjectionConstructor]
            public MockInvalidDualConstructorAttributes(int i) {}
        }

        class MockUndecoratedObject
        {
            public object Foo;

            [InjectionConstructor]
            public MockUndecoratedObject(object foo)
            {
                Foo = foo;
            }
        }

        class MockUndecoratedConstructorObject
        {
            public object Foo;

            public MockUndecoratedConstructorObject(object foo)
            {
                Foo = foo;
            }
        }
    }
}