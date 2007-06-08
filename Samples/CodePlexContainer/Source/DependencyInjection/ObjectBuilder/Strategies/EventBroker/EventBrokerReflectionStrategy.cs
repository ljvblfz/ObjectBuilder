using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class EventBrokerReflectionStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            EventBrokerPolicy policy = new EventBrokerPolicy();

            RegisterSinks(policy, typeToBuild);
            RegisterSources(policy, typeToBuild);

            if (!policy.IsEmpty)
                context.Policies.Set<IEventBrokerPolicy>(policy, typeToBuild, idToBuild);

            return base.BuildUp(context, typeToBuild, existing, idToBuild);
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