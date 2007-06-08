using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class EventBrokerService : IDisposable
    {
        // Fields

        ListDictionary<string, EventSource> sources = new ListDictionary<string, EventSource>();
        ListDictionary<string, EventSink> sinks = new ListDictionary<string, EventSink>();

        // Lifetime

        public void Dispose()
        {
            foreach (KeyValuePair<string, List<EventSource>> kvp in sources)
                foreach (EventSource source in kvp.Value)
                    source.Dispose();
        }

        // Methods

        public void Fire(string eventName,
                         object sender,
                         EventArgs e)
        {
            List<Exception> exceptions = new List<Exception>();

            foreach (EventSink sink in sinks[eventName])
            {
                Exception ex = sink.Invoke(sender, e);

                if (ex != null)
                    exceptions.Add(ex);
            }

            if (exceptions.Count > 0)
                throw new EventBrokerException(exceptions);
        }

        public void RegisterSink(object sink,
                                 MethodInfo methodInfo,
                                 string eventName)
        {
            Guard.ArgumentNotNull(sink, "sink");
            Guard.ArgumentNotNull(methodInfo, "methodInfo");
            Guard.ArgumentNotNullOrEmptyString(eventName, "eventName");

            sinks.Add(eventName, new EventSink(sink, methodInfo));
        }

        public void RegisterSource(object source,
                                   EventInfo eventInfo,
                                   string eventName)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(eventInfo, "eventInfo");
            Guard.ArgumentNotNullOrEmptyString(eventName, "eventName");

            sources.Add(eventName, new EventSource(this, source, eventInfo, eventName));
        }

        public void UnregisterSink(object sink,
                                   string eventName)
        {
            Guard.ArgumentNotNull(sink, "sink");
            Guard.ArgumentNotNullOrEmptyString(eventName, "eventName");

            List<EventSink> matchingSinks = new List<EventSink>();

            matchingSinks.AddRange(sinks.FindByKeyAndValue(delegate(string name)
                                                           {
                                                               return name == eventName;
                                                           },
                                                           delegate(EventSink snk)
                                                           {
                                                               return snk.Sink == sink;
                                                           }));

            foreach (EventSink eventSink in matchingSinks)
                sinks.Remove(eventName, eventSink);
        }

        public void UnregisterSource(object source,
                                     string eventName)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNullOrEmptyString(eventName, "eventName");

            List<EventSource> matchingSources = new List<EventSource>();

            matchingSources.AddRange(sources.FindByKeyAndValue(delegate(string name)
                                                               {
                                                                   return name == eventName;
                                                               },
                                                               delegate(EventSource src)
                                                               {
                                                                   return src.Source == source;
                                                               }));

            foreach (EventSource eventSource in matchingSources)
            {
                eventSource.Dispose();
                sources.Remove(eventName, eventSource);
            }
        }

        // Inner types

        class EventSink
        {
            // Fields

            WeakReference sink;
            MethodInfo methodInfo;
            Type handlerEventArgsType;

            // Lifetime

            public EventSink(object sink,
                             MethodInfo methodInfo)
            {
                this.sink = new WeakReference(sink);
                this.methodInfo = methodInfo;

                ParameterInfo[] parameters = methodInfo.GetParameters();

                if (parameters.Length != 2 || !typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType))
                    throw new ArgumentException("Method does not appear to be a valid event handler", "methodInfo");

                handlerEventArgsType = typeof(EventHandler<>).MakeGenericType(parameters[1].ParameterType);
            }

            // Properties

            public object Sink
            {
                get { return sink.Target; }
            }

            // Methods

            public Exception Invoke(object sender,
                                    EventArgs e)
            {
                object sinkObject = sink.Target;

                try
                {
                    if (sinkObject != null)
                    {
                        Delegate @delegate = Delegate.CreateDelegate(handlerEventArgsType, sinkObject, methodInfo);
                        @delegate.DynamicInvoke(sender, e);
                    }

                    return null;
                }
                catch (TargetInvocationException ex)
                {
                    return ex.InnerException;
                }
            }
        }

        class EventSource : IDisposable
        {
            // Fields

            EventBrokerService service;
            object source;
            EventInfo eventInfo;
            string eventName;
            Delegate @delegate;

            // Lifetime

            public EventSource(EventBrokerService service,
                               object source,
                               EventInfo eventInfo,
                               string eventName)
            {
                this.service = service;
                this.source = source;
                this.eventInfo = eventInfo;
                this.eventName = eventName;

                MethodInfo handler = GetType().GetMethod("SourceHandler");
                @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, handler);
                eventInfo.AddEventHandler(source, @delegate);
            }

            public void Dispose()
            {
                eventInfo.RemoveEventHandler(source, @delegate);
            }

            // Properties

            public object Source
            {
                get { return source; }
            }

            // Methods

            public void SourceHandler(object sender,
                                      EventArgs e)
            {
                service.Fire(eventName, sender, e);
            }
        }
    }
}