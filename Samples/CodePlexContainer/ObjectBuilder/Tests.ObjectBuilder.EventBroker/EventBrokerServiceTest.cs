using System;
using System.Reflection;
using Xunit;

namespace ObjectBuilder
{
    public class EventBrokerServiceTest
    {
        public class AcceptanceTests
        {
            [Test]
            public void ExceptionsAreCollectedAndRethrown()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSource source = new SpyEventSource();
                ExceptionThrowingSink sink1 = new ExceptionThrowingSink();
                ExceptionThrowingSink sink2 = new ExceptionThrowingSink();
                EventInfo sourceEvent = source.GetType().GetEvent("MySource");
                MethodInfo sinkMethod = sink1.GetType().GetMethod("MySink");
                service.RegisterSink(sink1, sinkMethod, "MyEvent");
                service.RegisterSink(sink2, sinkMethod, "MyEvent");
                service.RegisterSource(source, sourceEvent, "MyEvent");

                EventBrokerException ex =
                    Assert.Throws<EventBrokerException>(delegate
                                                        {
                                                            source.InvokeMySource();
                                                        });

                Assert.Equal(2, ex.Exceptions.Count);
            }

            [Test]
            public void RegistrationSinkFirst()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSource source = new SpyEventSource();
                SpyEventSink sink = new SpyEventSink();
                EventInfo sourceEvent = source.GetType().GetEvent("MySource");
                MethodInfo sinkMethod = sink.GetType().GetMethod("MySink");
                service.RegisterSink(sink, sinkMethod, "MyEvent");
                service.RegisterSource(source, sourceEvent, "MyEvent");

                source.InvokeMySource();

                Assert.Equal(source.SourceText, sink.EventValue);
            }

            [Test]
            public void RegistrationSourceFirst()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSource source = new SpyEventSource();
                SpyEventSink sink = new SpyEventSink();
                EventInfo sourceEvent = source.GetType().GetEvent("MySource");
                MethodInfo sinkMethod = sink.GetType().GetMethod("MySink");
                service.RegisterSource(source, sourceEvent, "MyEvent");
                service.RegisterSink(sink, sinkMethod, "MyEvent");

                source.InvokeMySource();

                Assert.Equal(source.SourceText, sink.EventValue);
            }
        }

        internal class ExceptionThrowingSink
        {
            public void MySink(object sender,
                               EventArgs<string> e)
            {
                throw new Exception("No thanks");
            }
        }

        public class RegisterSink
        {
            [Test]
            public void InvalidMethodSignature()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSink sink = new SpyEventSink();
                MethodInfo sinkMethod = sink.GetType().GetMethod("NonSinkMethod");

                Assert.Throws<ArgumentException>(delegate
                                                 {
                                                     service.RegisterSink(sink, sinkMethod, "MyEvent");
                                                 });
            }

            [Test]
            public void NullEventID()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSink sink = new SpyEventSink();
                MethodInfo sinkMethod = sink.GetType().GetMethod("MySink");

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         service.RegisterSink(sink, sinkMethod, null);
                                                     });
            }

            [Test]
            public void NullMethod()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSink sink = new SpyEventSink();

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         service.RegisterSink(sink, null, "MyEvent");
                                                     });
            }

            [Test]
            public void NullSink()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSink sink = new SpyEventSink();
                MethodInfo sinkMethod = sink.GetType().GetMethod("MySink");

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         service.RegisterSink(null, sinkMethod, "MyEvent");
                                                     });
            }
        }

        public class RegisterSource
        {
            [Test]
            public void NullEvent()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSource source = new SpyEventSource();

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         service.RegisterSource(source, null, "MyEvent");
                                                     });
            }

            [Test]
            public void NullEventID()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSource source = new SpyEventSource();
                EventInfo sourceEvent = source.GetType().GetEvent("MySource");

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         service.RegisterSource(source, sourceEvent, null);
                                                     });
            }

            [Test]
            public void NullSource()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSource source = new SpyEventSource();
                EventInfo sourceEvent = source.GetType().GetEvent("MySource");

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         service.RegisterSource(null, sourceEvent, "MyEvent");
                                                     });
            }
        }

        internal class SpyEventSink
        {
            public string EventValue;
            public bool WasCalled;

            public void MySink(object sender,
                               EventArgs<string> e)
            {
                WasCalled = true;
                EventValue = e.Data;
            }

            public void NonSinkMethod() {}
        }

        internal class SpyEventSource
        {
            public static bool FinalizerCalled;
            public string SourceText = "Hello, world!";

            public bool HasHandlers
            {
                get { return MySource != null; }
            }

            ~SpyEventSource()
            {
                FinalizerCalled = true;
            }

            public void InvokeMySource()
            {
                if (MySource != null)
                    MySource(this, new EventArgs<string>(SourceText));
            }

            public event EventHandler<EventArgs<string>> MySource;
        }

        public class UnregisterSink
        {
            [Test]
            public void NullEventID()
            {
                EventBrokerService service = new EventBrokerService();

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         service.UnregisterSink(new object(), null);
                                                     });
            }

            [Test]
            public void NullSink()
            {
                EventBrokerService service = new EventBrokerService();

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         service.UnregisterSink(null, "MyEvent");
                                                     });
            }

            [Test]
            public void UnregisterSinkUnwiresHandler()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSource source = new SpyEventSource();
                SpyEventSink sink = new SpyEventSink();
                EventInfo sourceEvent = source.GetType().GetEvent("MySource");
                MethodInfo sinkMethod = sink.GetType().GetMethod("MySink");
                service.RegisterSource(source, sourceEvent, "MyEvent");
                service.RegisterSink(sink, sinkMethod, "MyEvent");
                service.UnregisterSink(sink, "MyEvent");

                source.InvokeMySource();

                Assert.False(sink.WasCalled);
            }
        }

        public class UnregisterSource
        {
            [Test]
            public void NullEventID()
            {
                EventBrokerService service = new EventBrokerService();

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         service.UnregisterSink(new object(), null);
                                                     });
            }

            [Test]
            public void NullSource()
            {
                EventBrokerService service = new EventBrokerService();

                Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         service.UnregisterSource(null, "MyEvent");
                                                     });
            }

            [Test]
            public void UnregisterSourceUnwiresHandler()
            {
                EventBrokerService service = new EventBrokerService();
                SpyEventSource source = new SpyEventSource();
                EventInfo sourceEvent = source.GetType().GetEvent("MySource");
                service.RegisterSource(source, sourceEvent, "MyEvent");

                service.UnregisterSource(source, "MyEvent");

                Assert.False(source.HasHandlers);
            }
        }

        public class WeakReferences
        {
            [Test]
            public void SinksAreStoredWithWeakReferences()
            {
                EventBrokerService service = new EventBrokerService();
                MethodInfo sinkMethod = typeof(ExceptionThrowingSink).GetMethod("MySink");
                service.RegisterSink(new ExceptionThrowingSink(), sinkMethod, "MyEvent");

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.DoesNotThrow(delegate
                                    {
                                        service.Fire("MyEvent", this, new EventArgs<string>("Hello world"));
                                    });
            }

            [Test]
            public void SourcesAreStoredWithWeakReferences()
            {
                SpyEventSource.FinalizerCalled = false;
                EventBrokerService service = new EventBrokerService();
                EventInfo sourceEvent = typeof(SpyEventSource).GetEvent("MySource");
                service.RegisterSource(new SpyEventSource(), sourceEvent, "MyEvent");

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.True(SpyEventSource.FinalizerCalled);
            }
        }
    }
}