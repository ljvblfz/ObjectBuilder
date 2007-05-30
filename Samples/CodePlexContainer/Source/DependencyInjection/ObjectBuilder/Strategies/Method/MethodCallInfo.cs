using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class MethodCallInfo : IMethodCallInfo
    {
        // Fields

        MethodInfo method;
        string methodName;
        List<IParameter> parameters;

        // Lifetime

        public MethodCallInfo(string methodName)
            : this(methodName, (MethodInfo)null, null) {}

        public MethodCallInfo(string methodName,
                              params object[] parameters)
            : this(methodName, null, ObjectsToIParameters(parameters)) {}

        public MethodCallInfo(string methodName,
                              params IParameter[] parameters)
            : this(methodName, null, parameters) {}

        public MethodCallInfo(string methodName,
                              IEnumerable<IParameter> parameters)
            : this(methodName, null, parameters) {}

        public MethodCallInfo(MethodInfo method)
            : this(null, method, null) {}

        public MethodCallInfo(MethodInfo method,
                              params IParameter[] parameters)
            : this(null, method, parameters) {}

        public MethodCallInfo(MethodInfo method,
                              IEnumerable<IParameter> parameters)
            : this(null, method, parameters) {}

        MethodCallInfo(string methodName,
                       MethodInfo method,
                       IEnumerable<IParameter> parameters)
        {
            this.methodName = methodName;
            this.method = method;
            this.parameters = new List<IParameter>();

            if (parameters != null)
                foreach (IParameter param in parameters)
                    this.parameters.Add(param);
        }

        // Methods

        public object[] GetParameters(IBuilderContext context,
                                      Type type,
                                      string id,
                                      MethodInfo method)
        {
            List<object> values = new List<object>();

            foreach (IParameter param in parameters)
                values.Add(param.GetValue(context));

            return values.ToArray();
        }

        static IEnumerable<IParameter> ObjectsToIParameters(object[] parameters)
        {
            List<IParameter> results = new List<IParameter>();

            if (parameters != null)
                foreach (object parameter in parameters)
                    results.Add(new ValueParameter(parameter.GetType(), parameter));

            return results.ToArray();
        }

        public MethodInfo SelectMethod(IBuilderContext context,
                                       Type type,
                                       string id)
        {
            if (method != null)
                return method;

            List<Type> types = new List<Type>();

            foreach (IParameter param in parameters)
                types.Add(param.GetParameterType(context));

            return type.GetMethod(methodName, types.ToArray());
        }
    }
}