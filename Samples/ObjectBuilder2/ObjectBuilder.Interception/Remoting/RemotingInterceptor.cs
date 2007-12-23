using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public class RemotingInterceptor
    {
        public static object Wrap(object obj,
                                  Type typeToWrap,
                                  IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers)
        {
            return new RemotingProxy(obj, typeToWrap, handlers).GetTransparentProxy();
        }

        public static T Wrap<T>(T obj,
                                IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>> handlers)
        {
            return (T)Wrap(obj, typeof(T), handlers);
        }
    }
}