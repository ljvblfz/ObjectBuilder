using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class NamedMethodCallInfo : IMethodCallInfo
    {
        readonly string methodName;
        readonly List<IParameter> parameters;

        public NamedMethodCallInfo(string methodName,
                                   params IParameter[] parameters)
            : this(methodName, (IEnumerable<IParameter>)parameters) {}

        public NamedMethodCallInfo(string methodName,
                                   IEnumerable<IParameter> parameters)
        {
            this.methodName = methodName;
            this.parameters = new List<IParameter>();

            if (parameters != null)
                this.parameters.AddRange(parameters);
        }

        public void Execute(IBuilderContext context,
                            object instance,
                            object buildKey)
        {
            List<Type> parameterTypes = new List<Type>();
            foreach (IParameter parameter in parameters)
                parameterTypes.Add(parameter.GetParameterType(context));

            List<object> parameterValues = new List<object>();
            foreach (IParameter parameter in parameters)
                parameterValues.Add(parameter.GetValue(context));

            MethodInfo method = instance.GetType().GetMethod(methodName, parameterTypes.ToArray());
            method.Invoke(instance, parameterValues.ToArray());
        }
    }
}