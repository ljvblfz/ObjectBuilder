using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class CloneParameter : IParameter
    {
        readonly IParameter param;

        public CloneParameter(IParameter param)
        {
            this.param = param;
        }

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