using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class EventBrokerStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            IEventBrokerPolicy policy = context.Policies.Get<IEventBrokerPolicy>(existing.GetType(), idToBuild);
            EventBrokerService service = context.Locator.Get<EventBrokerService>();

            if (policy != null && service != null && context.Locator != null && context.Lifetime != null)
            {
                foreach (KeyValuePair<string, MethodInfo> kvp in policy.Sinks)
                    service.RegisterSink(existing, kvp.Value, kvp.Key);

                foreach (KeyValuePair<string, EventInfo> kvp in policy.Sources)
                    service.RegisterSource(existing, kvp.Value, kvp.Key);
            }

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }
    }
}