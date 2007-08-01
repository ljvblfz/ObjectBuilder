using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class InterfaceInterceptAttribute : InterceptAttribute
    {
        public InterfaceInterceptAttribute(Type handlerType)
            : base(handlerType) {}

        public override Type PolicyType
        {
            get { return typeof(InterfaceInterceptionPolicy); }
        }

        public override MethodBase GetMethodBaseForPolicy(Type typeRequested,
                                                          MethodBase method)
        {
            if (!typeRequested.IsInterface)
                return null;

            ParameterInfo[] paramInfos = method.GetParameters();
            object[] paramTypes = new object[paramInfos.Length];

            for (int idx = 0; idx < paramInfos.Length; ++idx)
                if (paramInfos[idx].ParameterType.IsGenericParameter)
                    paramTypes[idx] = paramInfos[idx].ParameterType.Name;
                else
                    paramTypes[idx] = paramInfos[idx].ParameterType;

            return InterfaceInterceptor.FindMethod(method.Name, paramTypes, typeRequested.GetMethods());
        }

        public override void ValidateInterceptionForType(Type typeRequested,
                                                         Type typeBeingBuilt)
        {
            if (!typeRequested.IsInterface)
                return;

            if (!typeRequested.IsPublic && !typeRequested.IsNestedPublic)
                throw new InvalidOperationException("Interface " + typeRequested.FullName + " must be public to be intercepted");
        }
    }
}