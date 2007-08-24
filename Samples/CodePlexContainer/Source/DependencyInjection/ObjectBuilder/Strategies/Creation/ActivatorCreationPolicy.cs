using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ActivatorCreationPolicy : ICreationPolicy
    {
        readonly List<IParameter> parameters;

        public ActivatorCreationPolicy(params IParameter[] parameters)
            : this((IEnumerable<IParameter>)parameters) {}

        public ActivatorCreationPolicy(IEnumerable<IParameter> parameters)
        {
            this.parameters = new List<IParameter>();

            if (parameters != null)
                this.parameters.AddRange(parameters);
        }

        public bool SupportsReflection
        {
            get { return false; }
        }

        public object Create(IBuilderContext context,
                             object buildKey)
        {
            Guard.ArgumentNotNull(context, "context");

            return Activator.CreateInstance(BuilderStrategy.GetTypeFromBuildKey(buildKey),
                                            GetParameters(context, null));
        }

        public ConstructorInfo GetConstructor(IBuilderContext context,
                                              object buildKey)
        {
            throw new NotImplementedException();
        }

        public object[] GetParameters(IBuilderContext context,
                                      ConstructorInfo constructor)
        {
            object[] result = new object[parameters.Count];

            for (int idx = 0; idx < parameters.Count; idx++)
                result[idx] = parameters[idx].GetValue(context);

            return result;
        }
    }
}