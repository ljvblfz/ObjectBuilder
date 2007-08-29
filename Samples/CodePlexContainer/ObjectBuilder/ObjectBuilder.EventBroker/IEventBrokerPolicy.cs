using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public interface IEventBrokerPolicy : IBuilderPolicy
    {
        IEnumerable<KeyValuePair<string, MethodInfo>> Sinks { get; }
        IEnumerable<KeyValuePair<string, EventInfo>> Sources { get; }
    }
}