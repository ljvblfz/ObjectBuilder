using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IEventBrokerPolicy : IBuilderPolicy
    {
        IEnumerable<KeyValuePair<string, MethodInfo>> Sinks { get; }
        IEnumerable<KeyValuePair<string, EventInfo>> Sources { get; }
    }
}