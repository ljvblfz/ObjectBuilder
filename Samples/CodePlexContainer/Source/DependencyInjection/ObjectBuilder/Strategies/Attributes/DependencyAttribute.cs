using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class DependencyAttribute : ParameterAttribute
    {
        Type createType;
        string name;
        NotPresentBehavior notPresentBehavior = NotPresentBehavior.CreateNew;

        public Type CreateType
        {
            get { return createType; }
            set { createType = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public NotPresentBehavior NotPresentBehavior
        {
            get { return notPresentBehavior; }
            set { notPresentBehavior = value; }
        }

        public override IParameter CreateParameter(Type annotatedMemberType)
        {
            return new DependencyParameter(annotatedMemberType, name, createType, notPresentBehavior);
        }
    }
}