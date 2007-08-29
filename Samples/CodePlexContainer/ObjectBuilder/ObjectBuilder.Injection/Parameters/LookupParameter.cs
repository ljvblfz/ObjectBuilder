using System;

namespace ObjectBuilder
{
    public class LookupParameter : IParameter
    {
        readonly object key;

        public LookupParameter(object key)
        {
            this.key = key;
        }

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