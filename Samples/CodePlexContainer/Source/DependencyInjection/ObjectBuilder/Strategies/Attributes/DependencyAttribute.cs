using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class DependencyAttribute : ParameterAttribute
    {
        readonly object buildKey;
        NotPresentBehavior notPresentBehavior = NotPresentBehavior.Build;

        public DependencyAttribute()
            : this(null) {}

        public DependencyAttribute(object buildKey)
        {
            this.buildKey = buildKey;
        }

        public object BuildKey
        {
            get { return buildKey; }
        }

        public NotPresentBehavior NotPresentBehavior
        {
            get { return notPresentBehavior; }
            set { notPresentBehavior = value; }
        }

        public override IParameter CreateParameter(Type annotatedMemberType)
        {
            return new DependencyParameter(BuildKey ?? annotatedMemberType, notPresentBehavior);
        }
    }
}