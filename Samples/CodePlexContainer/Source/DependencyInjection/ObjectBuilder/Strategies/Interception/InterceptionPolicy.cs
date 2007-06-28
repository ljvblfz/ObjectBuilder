using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class InterceptionPolicy : IInterceptionPolicy
    {
        readonly Dictionary<MethodBase, List<IInterceptionHandler>> handlers;
        readonly InterceptionType interceptionType;

        public InterceptionPolicy(InterceptionType interceptionType)
        {
            this.interceptionType = interceptionType;

            handlers = new Dictionary<MethodBase, List<IInterceptionHandler>>();
        }

        public int Count
        {
            get { return handlers.Count; }
        }

        public InterceptionType InterceptionType
        {
            get { return interceptionType; }
        }

        public IList<IInterceptionHandler> this[MethodBase method]
        {
            get { return handlers[method].AsReadOnly(); }
        }

        public IEnumerable<MethodBase> Methods
        {
            get { return handlers.Keys; }
        }

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