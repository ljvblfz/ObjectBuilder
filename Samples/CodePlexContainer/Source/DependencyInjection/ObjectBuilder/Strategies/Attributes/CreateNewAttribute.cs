using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class CreateNewAttribute : ParameterAttribute
    {
        public override IParameter CreateParameter(Type annotatedMemberType)
        {
            return new CreationParameter(annotatedMemberType, Guid.NewGuid().ToString());
        }
    }
}