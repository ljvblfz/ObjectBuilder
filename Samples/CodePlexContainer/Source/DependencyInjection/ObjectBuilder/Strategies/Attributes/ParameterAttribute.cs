using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class ParameterAttribute : Attribute
    {
        public abstract IParameter CreateParameter(Type annotatedMemberType);
    }
}