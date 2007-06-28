using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class MethodExecutionStrategy : BuilderStrategy
    {
        static void ApplyPolicy(IBuilderContext context,
                                object obj,
                                string id)
        {
            if (obj == null)
                return;

            Type type = obj.GetType();
            IMethodPolicy policy = context.Policies.Get<IMethodPolicy>(type, id);

            if (policy == null)
                return;

            foreach (IMethodCallInfo methodCallInfo in policy.Methods.Values)
            {
                MethodInfo methodInfo = methodCallInfo.SelectMethod(context, type, id);

                if (methodInfo != null)
                {
                    object[] parameters = methodCallInfo.GetParameters(context, type, id, methodInfo);
                    Guard.ValidateMethodParameters(methodInfo, parameters, obj.GetType());

                    methodInfo.Invoke(obj, parameters);
                }
            }
        }

        public override object BuildUp(IBuilderContext context,
                                       Type typeToBuild,
                                       object existing,
                                       string idToBuild)
        {
            ApplyPolicy(context, existing, idToBuild);
            return base.BuildUp(context, typeToBuild, existing, idToBuild);
        }
    }
}