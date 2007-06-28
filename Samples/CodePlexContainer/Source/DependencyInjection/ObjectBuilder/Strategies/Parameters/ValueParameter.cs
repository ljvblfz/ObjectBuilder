using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ValueParameter<TValue> : KnownTypeParameter
    {
        readonly TValue value;

        public ValueParameter(TValue value)
            : base(typeof(TValue))
        {
            this.value = value;
        }

        public override object GetValue(IBuilderContext context)
        {
            return value;
        }
    }

    public class ValueParameter : KnownTypeParameter
    {
        readonly object value;

        public ValueParameter(Type valueType,
                              object value)
            : base(valueType)
        {
            this.value = value;
        }

        public override object GetValue(IBuilderContext context)
        {
            return value;
        }
    }
}