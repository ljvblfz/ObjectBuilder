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

        static MethodInfo GetBaseTypeMethod(Type typeRequested,
                                            MethodBase method)
        {
            ParameterInfo[] paramInfos = method.GetParameters();
            Type[] paramTypes = new Type[paramInfos.Length];

            for (int idx = 0; idx < paramInfos.Length; ++idx)
                paramTypes[idx] = paramInfos[idx].ParameterType;

            return typeRequested.GetMethod(method.Name,
                                           BindingFlags.Public | BindingFlags.Instance,
                                           null,
                                           method.CallingConvention,
                                           paramTypes,
                                           null);
        }

        public override bool ShouldInterceptMethod(Type typeRequested,
                                                   MethodBase method)
        {
            if (!typeRequested.IsInterface)
                return false;

            return (GetBaseTypeMethod(typeRequested, method) != null);
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