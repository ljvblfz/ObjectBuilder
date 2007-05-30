using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class DependencyParameter : KnownTypeParameter
    {
        // Fields

        string name;
        Type createType;
        NotPresentBehavior notPresentBehavior;
        SearchMode searchMode;

        // Lifetime

        public DependencyParameter(Type parameterType,
                                   string name,
                                   Type createType,
                                   NotPresentBehavior notPresentBehavior,
                                   SearchMode searchMode)
            : base(parameterType)
        {
            this.name = name;
            this.createType = createType;
            this.notPresentBehavior = notPresentBehavior;
            this.searchMode = searchMode;
        }

        // Methods

        public override object GetValue(IBuilderContext context)
        {
            return new DependencyResolver(context).Resolve(
                base.type, createType, name, notPresentBehavior, searchMode);
        }
    }
}