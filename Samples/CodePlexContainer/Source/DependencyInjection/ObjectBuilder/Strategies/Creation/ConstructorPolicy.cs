using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ConstructorPolicy : ICreationPolicy
    {
        readonly ConstructorInfo constructor;
        readonly List<IParameter> parameters = new List<IParameter>();

        public ConstructorPolicy() {}

        public ConstructorPolicy(params IParameter[] parameters)
        {
            foreach (IParameter parameter in parameters)
                AddParameter(parameter);
        }

        public ConstructorPolicy(ConstructorInfo constructor,
                                 params IParameter[] parameters)
            : this(parameters)
        {
            this.constructor = constructor;
        }

        public void AddParameter(IParameter parameter)
        {
            parameters.Add(parameter);
        }

        public object[] GetParameters(IBuilderContext context,
                                      Type type,
                                      string id,
                                      ConstructorInfo ctor)
        {
            List<object> results = new List<object>();

            foreach (IParameter parm in parameters)
                results.Add(parm.GetValue(context));

            return results.ToArray();
        }

        public ConstructorInfo SelectConstructor(IBuilderContext context,
                                                 Type type,
                                                 string id)
        {
            if (constructor != null)
                return constructor;

            List<Type> types = new List<Type>();

            foreach (IParameter parm in parameters)
                types.Add(parm.GetParameterType(context));

            return type.GetConstructor(types.ToArray());
        }
    }
}