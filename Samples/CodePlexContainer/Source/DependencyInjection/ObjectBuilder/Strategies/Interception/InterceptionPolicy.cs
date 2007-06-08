using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class InterceptionPolicy : IInterceptionPolicy
    {
        // Fields

        Dictionary<MethodBase, List<IInterceptionHandler>> handlers;
        InterceptionType interceptionType;

        // Lifetime

        public InterceptionPolicy(InterceptionType interceptionType)
        {
            this.interceptionType = interceptionType;

            handlers = new Dictionary<MethodBase, List<IInterceptionHandler>>();
        }

        // Properties

        public IList<IInterceptionHandler> this[MethodBase method]
        {
            get { return handlers[method].AsReadOnly(); }
        }

        public int Count
        {
            get { return handlers.Count; }
        }

        public InterceptionType InterceptionType
        {
            get { return interceptionType; }
        }

        public IEnumerable<MethodBase> Methods
        {
            get { return handlers.Keys; }
        }

        // Methods

        public void Add(MethodBase method,
                        IEnumerable<IInterceptionHandler> methodHandlers)
        {
            handlers[method] = new List<IInterceptionHandler>(methodHandlers);
        }

        public void Add(MethodBase method,
                        params IInterceptionHandler[] methodHandlers)
        {
            handlers[method] = new List<IInterceptionHandler>(methodHandlers);
        }

        public IEnumerator<KeyValuePair<MethodBase, List<IInterceptionHandler>>> GetEnumerator()
        {
            return handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}