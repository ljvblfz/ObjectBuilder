using System;
using System.Threading;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace Ottawa
{
    public class OttawaContainerAcceptanceTests
    {
        [TestFixture]
        public class Dispose
        {
            [Test]
            public void DisposingContainerDisposesObjectsInContainer()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<DisposableObject>("");
                DisposableObject obj = container.Resolve<DisposableObject>();

                container.Dispose();

                Assert.True(obj.disposeCalled);
            }
        }

        [TestFixture]
        public class Errors
        {
            [Test]
            public void AddComponentWithNullName()
            {
                OttawaContainer container = new OttawaContainer();

                Assert.Throws<ArgumentNullException>(
                    delegate
                    {
                        container.AddComponent<object>(null);
                    });
            }

            [Test]
            public void AddComponentWithNullType()
            {
                OttawaContainer container = new OttawaContainer();

                Assert.Throws<ArgumentNullException>(
                    delegate
                    {
                        container.AddComponent("Foo", null);
                    });
            }

            [Test]
            public void AddMultipleComponentsForTheSameKey()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<object>("");

                Assert.Throws<ComponentRegistrationException>(
                    delegate
                    {
                        container.AddComponent<DateTime>("");
                    });
            }

            [Test]
            public void DefaultContainerDoesNotResolveAutomatically()
            {
                OttawaContainer container = new OttawaContainer();

                Assert.Throws<ComponentNotFoundException>(
                    delegate
                    {
                        container.Resolve<object>();
                    });
            }

            [Test]
            public void CannotResolveAbstractTypes()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<FooAbstract>("");

                Assert.Throws<ComponentActivatorException>(
                    delegate
                    {
                        container.Resolve<FooAbstract>();
                    });
            }

            [Test]
            public void CannotResolveInterfaces()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<IFoo>("");

                Assert.Throws<ComponentActivatorException>(
                    delegate
                    {
                        container.Resolve<IFoo>();
                    });
            }

            [Test]
            public void CannotResolveValueTypes()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<int>("");

                Assert.Throws<ComponentActivatorException>(
                    delegate
                    {
                        container.Resolve<int>();
                    });
            }
        }

        [TestFixture]
        public class Lifestyle
        {
            [Test]
            public void Singleton()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponentWithLifestyle<object>("", LifestyleType.Singleton);

                object obj1 = container.Resolve<object>("");
                object obj2 = container.Resolve<object>("");

                Assert.NotNull(obj1);
                Assert.NotNull(obj2);
                Assert.Same(obj1, obj2);
            }

            [Test]
            public void Transient()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponentWithLifestyle<object>("", LifestyleType.Transient);

                object obj1 = container.Resolve<object>("");
                object obj2 = container.Resolve<object>("");

                Assert.NotSame(obj1, obj2);
            }

            [Test, Ignore("Threading support not implemented yet")]
            public void CanResolveObjectsPerThread()
            {
                object obj1a = null, obj1b = null, obj2a, obj2b;
                OttawaContainer container = new OttawaContainer();
                container.AddComponentWithLifestyle<object>("", LifestyleType.Thread);

                Thread thread = new Thread(
                    (ThreadStart)delegate
                                 {
                                     obj1a = container.Resolve<object>();
                                     obj1b = container.Resolve<object>();
                                 });

                thread.Start();
                thread.Join();

                obj2a = container.Resolve<object>();
                obj2b = container.Resolve<object>();

                Assert.NotNull(obj1a);
                Assert.NotNull(obj2a);
                Assert.Same(obj1a, obj1b);
                Assert.Same(obj2a, obj2b);
                Assert.NotSame(obj1a, obj2a);
            }
        }

        [TestFixture]
        public class Release
        {
            [Test]
            public void ReleaseWithDefaultLifestyleIsNOOP()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<DisposableObject>("");
                DisposableObject obj = container.Resolve<DisposableObject>();

                container.Release(obj);
                DisposableObject obj2 = container.Resolve<DisposableObject>();

                Assert.False(obj.disposeCalled);
                Assert.Same(obj, obj2);
            }
        }

        [TestFixture]
        public class Resolve
        {
            [Test]
            public void CanResolveByKey()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<Foo>("");

                Foo result = container.Resolve<Foo>("");

                Assert.NotNull(result);
            }

            [Test]
            public void CanResolveByService()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<Foo>("");

                Foo result = container.Resolve<Foo>();

                Assert.NotNull(result);
            }

            [Test]
            public void ResolveForServiceTypeCreatesClassType()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<IFoo, Foo>("");

                IFoo result = container.Resolve<IFoo>();

                Assert.IsType<Foo>(result);
            }

            [Test]
            public void AddingTwoComponentsForTheSameServiceTypeResolvesToTheFirstOne()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<IFoo, Foo>("");
                container.AddComponent<IFoo, Foo2>("bob");

                IFoo result = container.Resolve<IFoo>();

                Assert.IsType<Foo>(result);
            }

            [Test]
            public void AddingTwoComponentAndServicesForTheSamKeyThrows()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<IFoo, Foo>("");

                Assert.Throws<ComponentRegistrationException>(
                    delegate
                    {
                        container.AddComponent<object, Foo2>("");
                    });
            }

            [Test]
            public void ServiceTypeNeedNotBeCompatibleWithClassType()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<IFoo, object>("");

                object result = container.Resolve(typeof(IFoo));

                Assert.NotNull(result);
                Assert.Null(result as IFoo);
            }

            [Test]
            public void SingletonsAreNotRecreatedAfterGarbageCollection()
            {
                Foo.ctorCount = 0;
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<Foo>("");

                container.Resolve<Foo>();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                container.Resolve<Foo>();

                Assert.Equal(1, Foo.ctorCount);
            }

            [Test]
            public void CanAddTheSameComponentUnderDifferentKeys()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<object>("");
                container.AddComponent<object>("foo");

                object resolvedEmpty = container.Resolve<object>("");
                object resolvedByName = container.Resolve<object>("foo");

                Assert.NotNull(resolvedEmpty);
                Assert.NotNull(resolvedByName);
            }

            [Test]
            public void SameTypeWithSameNameIsSameInstance()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<object>("");

                object result1 = container.Resolve<object>("");
                object result2 = container.Resolve<object>("");

                Assert.NotNull(result1);
                Assert.NotNull(result2);
                Assert.Same(result1, result2);
            }

            [Test]
            public void SameTypeWithTwoDifferentNamesIsTwoDifferenceInstances()
            {
                OttawaContainer container = new OttawaContainer();
                container.AddComponent<object>("");
                container.AddComponent<object>("foo");

                object resolvedEmpty = container.Resolve<object>("");
                object resolvedByName = container.Resolve<object>("foo");

                Assert.NotNull(resolvedEmpty);
                Assert.NotNull(resolvedByName);
                Assert.NotSame(resolvedEmpty, resolvedByName);
            }
        }

        class DisposableObject : IDisposable
        {
            public bool disposeCalled;

            public void Dispose()
            {
                disposeCalled = true;
            }
        }

        class Foo : IFoo
        {
            public static int ctorCount;

            public Foo()
            {
                ctorCount++;
            }
        }

        class Foo2 : IFoo {}

        abstract class FooAbstract {}

        interface IFoo {}
    }
}