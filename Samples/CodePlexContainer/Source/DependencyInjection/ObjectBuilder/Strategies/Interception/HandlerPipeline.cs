using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class HandlerPipeline
    {
        // Fields

        List<IInterceptionHandler> handlers;

        // Lifetime

        public HandlerPipeline(IEnumerable<IInterceptionHandler> handlers)
        {
            this.handlers = new List<IInterceptionHandler>(handlers);
        }

        public HandlerPipeline(params IInterceptionHandler[] handlers)
        {
            this.handlers = new List<IInterceptionHandler>(handlers);
        }

        // Methods

        public IMethodReturn Invoke(IMethodInvocation input,
                                    InvokeHandlerDelegate target)
        {
            if (handlers.Count == 0)
                return target(input, null);

            int handlerIndex = 0;

            IMethodReturn result = handlers[0].Invoke(input, delegate
                                                             {
                                                                 ++handlerIndex;

                                                                 if (handlerIndex < handlers.Count)
                                                                     return handlers[handlerIndex].Invoke;
                                                                 else
                                                                     return target;
                                                             });
            return result;
        }
    }
}