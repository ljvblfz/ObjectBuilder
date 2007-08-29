using System.Collections.Generic;
using System.Reflection;

namespace ObjectBuilder
{
    public class ReflectionMethodCallInfo : IMethodCallInfo
    {
        readonly MethodInfo method;
        readonly List<IParameter> parameters;

        public ReflectionMethodCallInfo(MethodInfo method,
                                        params IParameter[] parameters)
            : this(method, (IEnumerable<IParameter>)parameters) {}

        public ReflectionMethodCallInfo(MethodInfo method,
                                        IEnumerable<IParameter> parameters)
        {
            this.method = method;
            this.parameters = new List<IParameter>();

            if (parameters != null)
                Parameters.AddRange(parameters);
        }

        public MethodInfo Method
        {
            get { return method; }
        }

        public List<IParameter> Parameters
        {
            get { return parameters; }
        }

        public void Execute(IBuilderContext context,
                            object instance,
                            object buildKey)
        {
            List<object> parameterValues = new List<object>();
            foreach (IParameter parameter in Parameters)
                parameterValues.Add(parameter.GetValue(context));

            Method.Invoke(instance, parameterValues.ToArray());
        }
    }
}