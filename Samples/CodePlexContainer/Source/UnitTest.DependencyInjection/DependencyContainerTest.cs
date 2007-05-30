using CodePlex.DependencyInjection.ObjectBuilder;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection
{
    [TestFixture]
    public class DependencyContainerTest
    {
        [Test]
        public void ObjectsAreNotSingletonByDefault()
        {
            DependencyContainer container = new DependencyContainer();

            object obj1 = container.Get<object>();
            object obj2 = container.Get<object>();

            Assert.NotSame(obj1, obj2);
        }

        [Test]
        public void CanRegisterTypesToBeConsideredCached()
        {
            DependencyContainer container = new DependencyContainer();
            container.CacheInstancesOf<SingletonObject>();

            SingletonObject result1 = container.Get<SingletonObject>();
            SingletonObject result2 = container.Get<SingletonObject>();

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Same(result1, result2);
        }

        [Test]
        public void CanRegisterTypeMapping()
        {
            DependencyContainer container = new DependencyContainer();
            container.RegisterTypeMapping<ISingletonObject, SingletonObject>();

            ISingletonObject result = container.Get<ISingletonObject>();

            Assert.NotNull(result);
            Assert.IsType<SingletonObject>(result);
        }

        [Test]
        public void CanRegisterSingletonInstance()
        {
            DependencyContainer container = new DependencyContainer();
            SingletonObject obj = new SingletonObject();
            container.RegisterSingletonInstance<ISingletonObject>(obj);

            ISingletonObject result = container.Get<ISingletonObject>();

            Assert.Same(result, obj);
        }

        [Test]
        public void CanBuildExistingObject()
        {
            DependencyContainer container = new DependencyContainer();
            container.RegisterSingletonInstance("foo");
            ExistingObject existingObject = new ExistingObject();

            container.Inject(existingObject);

            Assert.Equal("foo", existingObject.Name);
        }

        [Test]
        public void NestedContainerCanReturnObjectsFromInnerContainer()
        {
            DependencyContainer innerContainer = new DependencyContainer();
            DependencyContainer outerContainer = new DependencyContainer(innerContainer);
            innerContainer.RegisterSingletonInstance("Hello world");

            string result = outerContainer.Get<string>();

            Assert.Equal("Hello world", result);
        }

        [Test]
        public void SettingTypeMappingOnInnerContainerAffectsOuterContainer()
        {
            DependencyContainer innerContainer = new DependencyContainer();
            DependencyContainer outerContainer = new DependencyContainer(innerContainer);
            innerContainer.RegisterTypeMapping<ISingletonObject, SingletonObject>();

            ISingletonObject result = outerContainer.Get<ISingletonObject>();

            Assert.IsType<SingletonObject>(result);
        }

        interface ISingletonObject {}

        class SingletonObject : ISingletonObject {}

        class ExistingObject
        {
            string name;

            [Dependency]
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
        }
    }
}