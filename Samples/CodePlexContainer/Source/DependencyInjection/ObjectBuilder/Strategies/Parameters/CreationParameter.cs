using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class CreationParameter : KnownTypeParameter
    {
        // Fields

        string creationID;

        // Lifetime

        public CreationParameter(Type type)
            : this(type, null) {}

        public CreationParameter(Type type,
                                 string id)
            : base(type)
        {
            creationID = id;
        }

        // Methods

        public override object GetValue(IBuilderContext context)
        {
            return context.HeadOfChain.BuildUp(context, type, null, creationID);
        }
    }
}