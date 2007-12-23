using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public class EventBrokerStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            IEventBrokerPolicy policy = context.Policies.Get<IEventBrokerPolicy>(buildKey);
            EventBrokerService service = context.Locator.Get<EventBrokerService>();

            if (policy != null && service != null)
            {
                foreach (KeyValuePair<string, MethodInfo> kvp in policy.Sinks)
                    service.RegisterSink(existing, kvp.Value, kvp.Key);

                foreach (KeyValuePair<string, EventInfo> kvp in policy.Sources)
                    service.RegisterSource(existing, kvp.Value, kvp.Key);
            }

            return base.BuildUp(context, buildKey, existing);
        }
    }
}