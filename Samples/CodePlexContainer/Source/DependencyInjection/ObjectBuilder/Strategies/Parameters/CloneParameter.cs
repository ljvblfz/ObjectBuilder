using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class CloneParameter : IParameter
    {
        // Fields

        IParameter param;

        // Lifetime

        public CloneParameter(IParameter param)
        {
            this.param = param;
        }

        // Methods

        public Type GetParameterType(IBuilderContext context)
        {
            return param.GetParameterType(context);
        }

        public object GetValue(IBuilderContext context)
        {
            object val = param.GetValue(context);

            if (val is ICloneable)
                val = ((ICloneable)val).Clone();

            return val;
        }
    }
}