using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public class EventBrokerPolicy : IEventBrokerPolicy
    {
        readonly Dictionary<string, MethodInfo> sinks = new Dictionary<string, MethodInfo>();
        readonly Dictionary<string, EventInfo> sources = new Dictionary<string, EventInfo>();

        public bool IsEmpty
        {
            get { return sinks.Count == 0 && sources.Count == 0; }
        }

        public IEnumerable<KeyValuePair<string, MethodInfo>> Sinks
        {
            get { return sinks; }
        }

        public IEnumerable<KeyValuePair<string, EventInfo>> Sources
        {
            get { return sources; }
        }

        public void AddSink(MethodInfo method,
                            string eventID)
        {
            sinks.Add(eventID, method);
        }

        public void AddSource(EventInfo @event,
                              string eventID)
        {
            sources.Add(eventID, @event);
        }
    }
}