using System;
using System.Globalization;
using CodePlex.DependencyInjection.Properties;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class DependencyResolver
    {
        // Fields

        IBuilderContext context;

        // Lifetime

        public DependencyResolver(IBuilderContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.context = context;
        }

        // Methods

        public object Resolve(Type typeToResolve,
                              Type typeToCreate,
                              string id,
                              NotPresentBehavior notPresent)
        {
            if (typeToResolve == null)
                throw new ArgumentNullException("typeToResolve");
            if (!Enum.IsDefined(typeof(NotPresentBehavior), notPresent))
                throw new ArgumentException(Resources.InvalidEnumerationValue, "notPresent");

            if (typeToCreate == null)
                typeToCreate = typeToResolve;

            DependencyResolutionLocatorKey key = new DependencyResolutionLocatorKey(typeToResolve, id);

            if (context.Locator.Contains(key))
                return context.Locator.Get(key);

            switch (notPresent)
            {
                case NotPresentBehavior.CreateNew:
                    return context.HeadOfChain.BuildUp(context, typeToCreate, null, key.ID);

                case NotPresentBehavior.ReturnNull:
                    return null;

                default:
                    throw new DependencyMissingException(
                        string.Format(CultureInfo.CurrentCulture,
                                      Resources.DependencyMissing, typeToResolve.ToString()));
            }
        }
    }
}