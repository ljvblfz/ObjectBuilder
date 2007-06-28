using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class DependencyParameter : KnownTypeParameter
    {
        readonly Type createType;
        readonly string name;
        readonly NotPresentBehavior notPresentBehavior;

        public DependencyParameter(Type parameterType,
                                   string name,
                                   Type createType,
                                   NotPresentBehavior notPresentBehavior)
            : base(parameterType)
        {
            this.name = name;
            this.createType = createType;
            this.notPresentBehavior = notPresentBehavior;
        }

        public override object GetValue(IBuilderContext context)
        {
            return new DependencyResolver(context).Resolve(type, createType, name, notPresentBehavior);
        }
    }
}