using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ConstructorCreationPolicy : ICreationPolicy
    {
        readonly ConstructorInfo constructor;
        readonly List<IParameter> parameters;

        public ConstructorCreationPolicy(ConstructorInfo constructor,
                                         params IParameter[] parameters)
            : this(constructor, (IEnumerable<IParameter>)parameters) {}

        public ConstructorCreationPolicy(ConstructorInfo constructor,
                                         IEnumerable<IParameter> parameters)
        {
            Guard.ArgumentNotNull(constructor, "constructor");

            this.constructor = constructor;
            this.parameters = new List<IParameter>();

            if (parameters != null)
                this.parameters.AddRange(parameters);
        }

        public bool SupportsReflection
        {
            get { return true; }
        }

        public object Create(IBuilderContext context,
                             object buildKey)
        {
            Guard.ArgumentNotNull(context, "context");

            return constructor.Invoke(GetParameters(context, constructor));
        }

        public ConstructorInfo GetConstructor(IBuilderContext context,
                                              object buildKey)
        {
            return constructor;
        }

        public object[] GetParameters(IBuilderContext context,
                                      ConstructorInfo ctor)
        {
            object[] result = new object[parameters.Count];

            for (int idx = 0; idx < parameters.Count; idx++)
                result[idx] = parameters[idx].GetValue(context);

            return result;
        }
    }
}