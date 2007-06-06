using System.Collections.Generic;

namespace CodePlex.DependencyInjection
{
    public class HandlerPipeline
    {
        // Fields

        List<ICallHandler> handlers;

        // Lifetime

        public HandlerPipeline(IEnumerable<ICallHandler> handlers)
        {
            this.handlers = new List<ICallHandler>(handlers);
        }

        public HandlerPipeline(params ICallHandler[] handlers)
        {
            this.handlers = new List<ICallHandler>(handlers);
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