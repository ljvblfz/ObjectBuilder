using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class DependencyParameter : KnownTypeParameter
    {
        // Fields

        string name;
        Type createType;
        NotPresentBehavior notPresentBehavior;

        // Lifetime

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

        // Methods

        public override object GetValue(IBuilderContext context)
        {
            return new DependencyResolver(context).Resolve(
                base.type, createType, name, notPresentBehavior);
        }
    }
}