using System;
using System.Collections.Generic;
using CodePlex.DependencyInjection.ObjectBuilder;
using NUnit.Framework;
using Assert=CodePlex.NUnitExtensions.Assert;

namespace CodePlex.DependencyInjection
{
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

        // Helpers

        interface IMyObject {}

        [TestFixture]
        public class MethodInterception
        {
            [Test]
            public void MustCallSetInterceptionTypeBeforeCallingIntercept()
            {
                DependencyContainer container = new DependencyContainer();

                Assert.Throws<InvalidOperationException>(
                    delegate
                    {
                        container.Intercept<object>(typeof(object).GetMethod("GetType"), new RecordingHandler());
                    });
            }

            [Test]
            public void SetInterceptionTypeTwiceWithDifferentTypes()
            {
                DependencyContainer container = new DependencyContainer();
                container.SetInterceptionType<object>(InterceptionType.Remoting);

                Assert.Throws<ArgumentException>(
                    delegate
                    {
                        container.SetInterceptionType<object>(InterceptionType.VirtualMethod);
                    });
            }

            [Test]
            public void SetInterceptionTypeTwiceWithSameTypeDoesNotThrow()
            {
                DependencyContainer container = new DependencyContainer();
                container.SetInterceptionType<object>(InterceptionType.Remoting);
                container.SetInterceptionType<object>(InterceptionType.Remoting);
            }

            [Test]
            public void RemotingInterceptionViaCode()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();
                container.SetInterceptionType<SpyMBRO>(InterceptionType.Remoting);
                container.Intercept<SpyMBRO>(typeof(SpyMBRO).GetMethod("InterceptedMethod"),
                                             new RecordingHandler());

                SpyMBRO obj = container.Get<SpyMBRO>();
                obj.InterceptedMethod();

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            [Test]
            public void RemotingInterceptionViaAttributes()
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
            public void RemotingInterceptionThroughInterfaceViaAttributes()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();
                container.RegisterTypeMapping<ISpy, SpyInterfaceImplWithAttribute>();

                ISpy obj = container.Get<ISpy>();
                obj.InterceptedMethod();

                Assert.Equal(4, Recorder.Records.Count);
                Assert.Equal("OnBuiltUp", Recorder.Records[0]);
                Assert.Equal("Before Method", Recorder.Records[1]);
                Assert.Equal("In Method", Recorder.Records[2]);
                Assert.Equal("After Method", Recorder.Records[3]);
            }

            [Test]
            public void CanInterceptWithVirtualMethodOverride()
            {
                Recorder.Records.Clear();
                DependencyContainer container = new DependencyContainer();
                container.SetInterceptionType<SpyVirtual>(InterceptionType.VirtualMethod);
                container.Intercept<SpyVirtual>(typeof(SpyVirtual).GetMethod("InterceptedMethod"),
                                                new RecordingHandler());

                SpyVirtual obj = container.Get<SpyVirtual>();
                obj.InterceptedMethod();

                Assert.Equal(3, Recorder.Records.Count);
                Assert.Equal("Before Method", Recorder.Records[0]);
                Assert.Equal("In Method", Recorder.Records[1]);
                Assert.Equal("After Method", Recorder.Records[2]);
            }

            // Helpers

            static class Recorder
            {
                public static List<string> Records = new List<string>();
            }

            class RecordingHandler : IInterceptionHandler
            {
                public IMethodReturn Invoke(IMethodInvocation call,
                                            GetNextHandlerDelegate getNext)
                {
                    Recorder.Records.Add("Before Method");
                    IMethodReturn result = getNext().Invoke(call, getNext);
                    Recorder.Records.Add("After Method");
                    return result;
                }
            }

            internal class SpyMBRO : MarshalByRefObject
            {
                public void InterceptedMethod()
                {
                    Recorder.Records.Add("In Method");
                }
            }

            [InterceptType(InterceptionType.Remoting)]
            internal class SpyMBROWithAttribute : MarshalByRefObject
            {
                [Intercept(typeof(RecordingHandler))]
                public void InterceptedMethod()
                {
                    Recorder.Records.Add("In Method");
                }
            }

            internal interface ISpy
            {
                void InterceptedMethod();
            }

            [InterceptType(InterceptionType.Remoting)]
            internal class SpyInterfaceImplWithAttribute : ISpy, IBuilderAware
            {
                [Intercept(typeof(RecordingHandler))]
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

            public class SpyVirtual
            {
                public virtual void InterceptedMethod()
                {
                    Recorder.Records.Add("In Method");
                }
            }
        }

        class MyObject : IMyObject {}

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