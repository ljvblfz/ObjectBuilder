using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class ValueParameter<TValue> : KnownTypeParameter
    {
        // Fields

        TValue value;

        // Lifetime

        public ValueParameter(TValue value)
            : base(typeof(TValue))
        {
            this.value = value;
        }

        // Methods

        public override object GetValue(IBuilderContext context)
        {
            return value;
        }
    }

    public class ValueParameter : KnownTypeParameter
    {
        // Fields

        object value;

        // Lifetime

        public ValueParameter(Type valueType,
                              object value)
            : base(valueType)
        {
            this.value = value;
        }

        // Methods

        public override object GetValue(IBuilderContext context)
        {
            return value;
        }
    }
}