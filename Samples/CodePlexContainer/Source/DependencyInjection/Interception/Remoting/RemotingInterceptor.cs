using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection
{
    public class RemotingInterceptor
    {
        public static object Wrap(object obj,
                                  Type typeToWrap,
                                  IEnumerable<KeyValuePair<MethodBase, List<ICallHandler>>> handlers)
        {
            return new RemotingProxy(obj, typeToWrap, handlers).GetTransparentProxy();
        }

        public static T Wrap<T>(T obj,
                                IEnumerable<KeyValuePair<MethodBase, List<ICallHandler>>> handlers)
        {
            return (T)Wrap(obj, typeof(T), handlers);
        }
    }
}