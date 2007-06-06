using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection
{
    public class InterceptionPolicy : IInterceptionPolicy
    {
        // Fields

        Dictionary<MethodBase, List<ICallHandler>> handlers;
        InterceptionType interceptionType;

        // Lifetime

        public InterceptionPolicy(InterceptionType interceptionType)
        {
            this.interceptionType = interceptionType;

            handlers = new Dictionary<MethodBase, List<ICallHandler>>();
        }

        // Properties

        public InterceptionType InterceptionType
        {
            get { return interceptionType; }
        }

        // Methods

        public void Add(MethodBase method,
                        IEnumerable<ICallHandler> methodHandlers)
        {
            handlers[method] = new List<ICallHandler>(methodHandlers);
        }

        public void Add(MethodBase method,
                        params ICallHandler[] methodHandlers)
        {
            handlers[method] = new List<ICallHandler>(methodHandlers);
        }

        public IEnumerator<KeyValuePair<MethodBase, List<ICallHandler>>> GetEnumerator()
        {
            return handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}