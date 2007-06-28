using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class CreationParameter : KnownTypeParameter
    {
        readonly string creationID;

        public CreationParameter(Type type)
            : this(type, null) {}

        public CreationParameter(Type type,
                                 string id)
            : base(type)
        {
            creationID = id;
        }

        public override object GetValue(IBuilderContext context)
        {
            return context.HeadOfChain.BuildUp(context, type, null, creationID);
        }
    }
}