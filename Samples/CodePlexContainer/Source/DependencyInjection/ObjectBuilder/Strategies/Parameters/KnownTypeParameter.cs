using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class KnownTypeParameter : IParameter
    {
        protected Type type;

        protected KnownTypeParameter(Type type)
        {
            this.type = type;
        }

        public Type GetParameterType(IBuilderContext context)
        {
            return type;
        }

        public abstract object GetValue(IBuilderContext context);
    }
}