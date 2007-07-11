using System;
using CodePlex.DependencyInjection.ObjectBuilder;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection
{
    interface IMyObject {}

    class MyObject : IMyObject {}

    public class DependencyContainerTest
    {
        [TestFixture]
        public class EventBroker
        {
            [Test]
            public void RegisterByCode()
            {
                DependencyContainer container = new DependencyContainer();
                container.RegisterEventSource<EventSourceCode>("TheEvent", "MyEventID");
                container.RegisterEventSink<EventSinkCode>("TheHandler", "MyEventID");
                EventSourceCode source = container.Get<EventSourceCode>();
                EventSinkCode sink = container.Get<EventSinkCode>();

                source.Invoke();

                Assert.NotNull(sink.HandlerArgs);
            }

            [Test]
            public void RegisterByAttributes()
            {
                DependencyContainer container = new DependencyContainer();
                EventSourceAttr source = container.Get<EventSourceAttr>();
                EventSinkAttr sink = container.Get<EventSinkAttr>();

                source.Invoke();

                Assert.NotNull(sink.HandlerArgs);
            }

            internal class EventSourceCode
            {
                public void Invoke()
                {
                    if (TheEvent != null)
                        TheEvent(this, EventArgs.Empty);
                }

                public event EventHandler<EventArgs> TheEvent;
            }

            internal class EventSourceAttr
            {
                public void Invoke()
                {
                    if (TheEvent != null)
                        TheEvent(this, EventArgs.Empty);
                }

                [EventSource("MyEvent")]
                public event EventHandler<EventArgs> TheEvent;
            }

            internal class EventSinkCode
            {
                public EventArgs HandlerArgs;

                public void TheHandler(object sender,
                                       EventArgs e)
                {
                    HandlerArgs = e;
                }
            }

            internal class EventSinkAttr
            {
                public EventArgs HandlerArgs;

                [EventSink("MyEvent")]
                public void TheHandler(object sender,
                                       EventArgs e)
                {
                    HandlerArgs = e;
                }
            }
        }

        [TestFixture]
        public class ExistingObjects
        {
            [Test]
            public void NullThrows()
            {
                DependencyContainer container = new DependencyContainer();

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         container.Inject(null);
                                                     });
            }

            [Test]
            public void CanInjectExistingObject()
            {
                ExistingObject obj = new ExistingObject();
                DependencyContainer container = new DependencyContainer();
                container.RegisterSingletonInstance("Hello world");

                container.Inject(obj);

                Assert.Equal("Hello world", obj.Name);
            }

            // Helpers

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

        [TestFixture]
        public class InterceptInterface
        {
            [Test]
            public void InterceptViaCode()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();
                container.RegisterTypeMapping<ISpy, SpyInterface>();
                container.InterceptInterface<SpyInterface>(typeof(ISpy).GetMethod("InterceptedMethod"),
                                                           new RecordingHandler());

                ISpy obj = container.Get<ISpy>();
                obj.InterceptedMethod();

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void InterceptViaAttributes()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();
                container.RegisterTypeMapping<ISpy, SpyInterfaceAttributes>();

                ISpy obj = container.Get<ISpy>();
                obj.InterceptedMethod();

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void ExceptionsAreUnchanged()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();
                container.RegisterTypeMapping<ISpy, SpyInterface>();
                container.InterceptInterface<SpyInterface>(typeof(ISpy).GetMethod("ThrowsException"),
                                                           new RecordingHandler());

                ISpy obj = container.Get<ISpy>();

                Assert.Throws<Exception>(delegate
                                         {
                                             obj.ThrowsException();
                                         });

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            internal sealed class SpyInterface : ISpy
            {
                public void InterceptedMethod()
                {
                    Recorder.Records.Add("In Method");
                }

                public void ThrowsException()
                {
                    Recorder.Records.Add("In Method");
                    throw new Exception();
                }
            }

            internal sealed class SpyInterfaceAttributes : ISpy
            {
                [InterfaceIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod()
                {
                    Recorder.Records.Add("In Method");
                }

                public void ThrowsException()
                {
                    throw new Exception();
                }
            }

            public interface ISpy
            {
                void InterceptedMethod();
                void ThrowsException();
            }
        }

        [TestFixture]
        public class InterceptInterface_GenericMethods
        {
            //[Test]
            //public void GenericMethodOnNonGenericInterface()
            //{
            //    Recorder.Records.Clear();
            //    DependencyContainer container = new DependencyContainer();
            //    container.RegisterTypeMapping<IFoo, Foo>();

            //    IFoo obj = container.Get<IFoo>();
            //    obj.Bar(42);
            //    obj.Bar("world");

            //    Assert.Equal(6, Recorder.Records.Count);
            //    Assert.Equal("Before Method", Recorder.Records[0]);
            //    Assert.Equal("Passed: 42", Recorder.Records[1]);
            //    Assert.Equal("After Method", Recorder.Records[2]);
            //    Assert.Equal("Before Method", Recorder.Records[3]);
            //    Assert.Equal("Passed: world", Recorder.Records[4]);
            //    Assert.Equal("After Method", Recorder.Records[5]);
            //}

            //public class DerivedFoo : IFoo {
            //    readonly ILEmitProxy proxy;
            //    readonly IFoo target;
            //    readonly MethodInfo method0;
            //    readonly Type[] method0GenericArgs;

            //    public DerivedFoo(ILEmitProxy proxy, IFoo target)
            //    {
            //        this.proxy = proxy;
            //        this.target = target;

            //        MethodInfo tempMethod = typeof(IFoo).GetMethod("Bar");
            //        method0 = tempMethod.GetGenericMethodDefinition();
            //        method0GenericArgs = tempMethod.GetGenericArguments();
            //    }

            //    public void Bar<T>(T value)
            //    {
            //        MethodInfo realMethod = method0.MakeGenericMethod(method0GenericArgs);
            //        proxy.Invoke(target, realMethod, new object[] { value }, Bar__Delegate);
            //    }

            //    public object Bar__Delegate(object[] arguments)
            //    {
            //        // What to write here???
            //        return null;
            //    }
            //}

            public interface IFoo
            {
                void Bar<T>(T value);
            }

            public class Foo : IFoo
            {
                [InterfaceIntercept(typeof(RecordingHandler))]
                public void Bar<T>(T value)
                {
                    Recorder.Records.Add("Passed " + value);
                }
            }

            public interface IGenericFoo<T>
            {
                void Bar(T value);
            }

            public class GenericFoo<T> : IGenericFoo<T>
            {
                public void Bar(T value)
                {
                    Recorder.Records.Add("Passed " + value);
                }
            }

            public class ConcreteFoo : IGenericFoo<int>
            {
                public void Bar(int value)
                {
                    Recorder.Records.Add("Passed " + value);
                }
            }
        }

        [TestFixture]
        public class InterceptInterface_Properties
        {
            [Test]
            public void CanInterceptPropertySettersAndGetters()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();
                container.RegisterTypeMapping<IFoo, Foo>();

                IFoo obj = container.Get<IFoo>();
                obj.MyProperty = "foo";
                string unused = obj.MyProperty;

                Assert.Equal(6, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("PropSet: foo", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
                Assert.Equal("Before Method", Recorder.Records[3]);
                Assert.Equal("PropGet", Recorder.Records[4]);
                Assert.Equal("After Method", Recorder.Records[5]);
            }

            public interface IFoo
            {
                string MyProperty { get; set; }
            }

            public class Foo : IFoo
            {
                public string MyProperty
                {
                    [InterfaceIntercept(typeof(RecordingHandler))]
                    get
                    {
                        Recorder.Records.Add("PropGet");
                        return null;
                    }

                    [InterfaceIntercept(typeof(RecordingHandler))]
                    set { Recorder.Records.Add("PropSet: " + value); }
                }
            }
        }

        [TestFixture]
        public class InterceptRemoting
        {
            [Test]
            public void InterceptViaCode()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();
                container.InterceptRemoting<SpyMBRO>(typeof(SpyMBRO).GetMethod("InterceptedMethod"),
                                                     new RecordingHandler());

                SpyMBRO obj = container.Get<SpyMBRO>();
                obj.InterceptedMethod();

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void InterceptViaAttributes()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();

                SpyMBROWithAttribute obj = container.Get<SpyMBROWithAttribute>();
                obj.InterceptedMethod();

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void InterceptInterfaceViaAttributes()
            {
                DependencyContainer container = new DependencyContainer();
                container.RegisterTypeMapping<ISpy, SpyInterfaceImplWithAttribute>();

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        container.Get<ISpy>();
                    });
            }

            internal class SpyMBRO : MarshalByRefObject
            {
                public void InterceptedMethod()
                {
                    Recorder.Records.Add("In Method");
                }
            }

            internal class SpyMBROWithAttribute : MarshalByRefObject
            {
                [RemotingIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod()
                {
                    Recorder.Records.Add("In Method");
                }
            }

            internal interface ISpy
            {
                void InterceptedMethod();
            }

            internal class SpyInterfaceImplWithAttribute : ISpy, IBuilderAware
            {
                [RemotingIntercept(typeof(RecordingHandler))]
                public void InterceptedMethod()
                {
                    Recorder.Records.Add("In Method");
                }

                public void OnBuiltUp(string id)
                {
                    Recorder.Records.Add("OnBuiltUp");
                }

                public void OnTearingDown()
                {
                    Recorder.Records.Add("OnTearingDown");
                }
            }
        }

        [TestFixture]
        public class InterceptVirtual
        {
            [Test]
            public void InterceptViaCode()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();
                container.InterceptVirtual<SpyVirtual>(typeof(SpyVirtual).GetMethod("InterceptedMethod"),
                                                       new RecordingHandler());

                SpyVirtual obj = container.Get<SpyVirtual>();
                obj.InterceptedMethod();
                obj.NonInterceptedMethod();

                Assert.Equal(4, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
                Assert.Equal("In Non-Intercepted Method", Recorder.Records[3]);
            }

            [Test]
            public void InterceptViaAttributes()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();

                SpyVirtualAttributes obj = container.Get<SpyVirtualAttributes>();
                obj.InterceptedMethod();

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void ExceptionsAreUnchanged()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();
                container.InterceptVirtual<SpyVirtual>(typeof(SpyVirtual).GetMethod("ThrowsException"),
                                                       new RecordingHandler());

                SpyVirtual obj = container.Get<SpyVirtual>();

                Assert.Throws<Exception>(delegate
                                         {
                                             obj.ThrowsException();
                                         });

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            public class SpyVirtual
            {
                public virtual void InterceptedMethod()
                {
                    Recorder.Records.Add("In Method");
                }

                public void NonInterceptedMethod()
                {
                    Recorder.Records.Add("In Non-Intercepted Method");
                }

                public virtual void ThrowsException()
                {
                    Recorder.Records.Add("In Method");
                    throw new Exception("This is my exception!");
                }
            }

            public class SpyVirtualAttributes
            {
                [VirtualIntercept(typeof(RecordingHandler))]
                public virtual void InterceptedMethod()
                {
                    Recorder.Records.Add("In Method");
                }
            }
        }

        [TestFixture]
        public class Singletons
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
                container.CacheInstancesOf<MyObject>();

                MyObject result1 = container.Get<MyObject>();
                MyObject result2 = container.Get<MyObject>();

                Assert.NotNull(result1);
                Assert.NotNull(result2);
                Assert.Same(result1, result2);
            }

            [Test]
            public void CanRegisterSingletonInstance()
            {
                DependencyContainer container = new DependencyContainer();
                MyObject obj = new MyObject();
                container.RegisterSingletonInstance<IMyObject>(obj);

                IMyObject result = container.Get<IMyObject>();

                Assert.Same(result, obj);
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
        }

        [TestFixture]
        public class TypeMapping
        {
            [Test]
            public void CanRegisterTypeMapping()
            {
                DependencyContainer container = new DependencyContainer();
                container.RegisterTypeMapping<IMyObject, MyObject>();

                IMyObject result = container.Get<IMyObject>();

                Assert.NotNull(result);
                Assert.IsType<MyObject>(result);
            }

            [Test]
            public void SettingTypeMappingOnInnerContainerAffectsOuterContainer()
            {
                DependencyContainer innerContainer = new DependencyContainer();
                DependencyContainer outerContainer = new DependencyContainer(innerContainer);
                innerContainer.RegisterTypeMapping<IMyObject, MyObject>();

                IMyObject result = outerContainer.Get<IMyObject>();

                Assert.IsType<MyObject>(result);
            }
        }
    }
}