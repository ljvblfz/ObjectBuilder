using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class LookupParameter : IParameter
    {
        // Fields

        object key;

        // Lifetime

        public LookupParameter(object key)
        {
            this.key = key;
        }

        // Methods

        public Type GetParameterType(IBuilderContext context)
        {
            return GetValue(context).GetType();
        }

        public object GetValue(IBuilderContext context)
        {
            return context.Locator.Get(key);
        }
    }
}