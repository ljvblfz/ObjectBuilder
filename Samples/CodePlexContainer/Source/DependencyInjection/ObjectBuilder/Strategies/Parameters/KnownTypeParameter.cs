using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class KnownTypeParameter : IParameter
    {
        // Fields

        protected Type type;

        // Lifetime

        protected KnownTypeParameter(Type type)
        {
            this.type = type;
        }

        // Methods

        public Type GetParameterType(IBuilderContext context)
        {
            return type;
        }

        public abstract object GetValue(IBuilderContext context);
    }
}