using System;
using System.Reflection;

namespace ObjectBuilder
{
    public class EventBrokerReflectionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            Type typeToBuild;

            if (TryGetTypeFromBuildKey(buildKey, out typeToBuild))
            {
                EventBrokerPolicy policy = new EventBrokerPolicy();

                RegisterSinks(policy, typeToBuild);
                RegisterSources(policy, typeToBuild);

                if (!policy.IsEmpty)
                    context.Policies.Set<IEventBrokerPolicy>(policy, buildKey);
            }

            return base.BuildUp(context, buildKey, existing);
        }

        static void RegisterSinks(EventBrokerPolicy policy,
                                  Type type)
        {
            foreach (MethodInfo method in type.GetMethods())
                foreach (EventSinkAttribute attr in method.GetCustomAttributes(typeof(EventSinkAttribute), true))
                    policy.AddSink(method, attr.Name);
        }

        static void RegisterSources(EventBrokerPolicy policy,
                                    Type type)
        {
            foreach (EventInfo @event in type.GetEvents())
                foreach (EventSourceAttribute attr in @event.GetCustomAttributes(typeof(EventSourceAttribute), true))
                    policy.AddSource(@event, attr.Name);
        }
    }
}