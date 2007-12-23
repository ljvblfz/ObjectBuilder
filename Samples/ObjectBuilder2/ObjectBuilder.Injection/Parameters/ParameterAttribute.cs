using System;

namespace ObjectBuilder
{
    public abstract class ParameterAttribute : Attribute
    {
        public abstract IParameter CreateParameter(Type annotatedMemberType);
    }
}