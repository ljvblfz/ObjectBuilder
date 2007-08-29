using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public class EventBrokerService : IDisposable
    {
        readonly ListDictionary<string, EventSink> sinks = new ListDictionary<string, EventSink>();
        readonly ListDictionary<string, EventSource> sources = new ListDictionary<string, EventSource>();

        public void Dispose()
        {
            foreach (KeyValuePair<string, List<EventSource>> kvp in sources)
                foreach (EventSource source in kvp.Value)
                    source.Dispose();
        }

        public void Fire(string eventID,
                         object sender,
                         EventArgs e)
        {
            List<Exception> exceptions = new List<Exception>();

            foreach (EventSink sink in sinks[eventID])
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
                                 string eventID)
        {
            Guard.ArgumentNotNull(sink, "sink");
            Guard.ArgumentNotNull(methodInfo, "methodInfo");
            Guard.ArgumentNotNullOrEmpty(eventID, "eventID");

            RemoveDeadSinksAndSources();

            sinks.Add(eventID, new EventSink(sink, methodInfo));
        }

        public void RegisterSource(object source,
                                   EventInfo eventInfo,
                                   string eventID)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(eventInfo, "eventInfo");
            Guard.ArgumentNotNullOrEmpty(eventID, "eventID");

            RemoveDeadSinksAndSources();

            sources.Add(eventID, new EventSource(this, source, eventInfo, eventID));
        }

        void RemoveDeadSinksAndSources()
        {
            foreach (string eventID in sinks.Keys)
                sinks[eventID].RemoveAll(delegate(EventSink sink)
                                         {
                                             return sink.Sink == null;
                                         });

            foreach (string eventID in sources.Keys)
                sources[eventID].RemoveAll(delegate(EventSource source)
                                           {
                                               return source.Source == null;
                                           });
        }

        public void UnregisterSink(object sink,
                                   string eventID)
        {
            Guard.ArgumentNotNull(sink, "sink");
            Guard.ArgumentNotNullOrEmpty(eventID, "eventID");

            RemoveDeadSinksAndSources();

            List<EventSink> matchingSinks = new List<EventSink>();

            matchingSinks.AddRange(sinks.FindByKeyAndValue(delegate(string name)
                                                           {
                                                               return name == eventID;
                                                           },
                                                           delegate(EventSink snk)
                                                           {
                                                               return snk.Sink == sink;
                                                           }));

            foreach (EventSink eventSink in matchingSinks)
                sinks.Remove(eventID, eventSink);
        }

        public void UnregisterSource(object source,
                                     string eventID)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNullOrEmpty(eventID, "eventID");

            RemoveDeadSinksAndSources();

            List<EventSource> matchingSources = new List<EventSource>();

            matchingSources.AddRange(sources.FindByKeyAndValue(delegate(string name)
                                                               {
                                                                   return name == eventID;
                                                               },
                                                               delegate(EventSource src)
                                                               {
                                                                   return src.Source == source;
                                                               }));

            foreach (EventSource eventSource in matchingSources)
            {
                eventSource.Dispose();
                sources.Remove(eventID, eventSource);
            }
        }

        internal class EventSink
        {
            readonly Type handlerEventArgsType;
            readonly MethodInfo methodInfo;
            readonly WeakReference sink;

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

            public object Sink
            {
                get { return sink.Target; }
            }

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

        internal class EventSource : IDisposable
        {
            readonly string eventID;
            readonly EventInfo eventInfo;
            readonly MethodInfo handlerMethod;
            readonly EventBrokerService service;
            readonly WeakReference source;

            public EventSource(EventBrokerService service,
                               object source,
                               EventInfo eventInfo,
                               string eventID)
            {
                this.service = service;
                this.source = new WeakReference(source);
                this.eventInfo = eventInfo;
                this.eventID = eventID;

                handlerMethod = GetType().GetMethod("SourceHandler");
                Delegate @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, handlerMethod);
                eventInfo.AddEventHandler(source, @delegate);
            }

            public object Source
            {
                get { return source.Target; }
            }

            public void Dispose()
            {
                object sourceObj = source.Target;

                if (sourceObj != null)
                {
                    Delegate @delegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, handlerMethod);
                    eventInfo.RemoveEventHandler(sourceObj, @delegate);
                }
            }

            public void SourceHandler(object sender,
                                      EventArgs e)
            {
                service.Fire(eventID, sender, e);
            }
        }
    }
}