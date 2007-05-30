using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    /// <summary>
    /// An implementation of <see cref="IBuilderStrategy"/>.
    /// </summary>
    public abstract class BuilderStrategy : IBuilderStrategy
    {
        /// <summary>
        /// Generic version of BuildUp, to aid unit-testing.
        /// </summary>
        public TItem BuildUp<TItem>(IBuilderContext context,
                                    TItem existing,
                                    string idToBuild)
        {
            return (TItem)BuildUp(context, typeof(TItem), existing, idToBuild);
        }

        /// <summary>
        /// See <see cref="IBuilderStrategy.BuildUp"/> for more information.
        /// </summary>
        public virtual object BuildUp(IBuilderContext context,
                                      Type typeToBuild,
                                      object existing,
                                      string idToBuild)
        {
            IBuilderStrategy next = context.GetNextInChain(this);

            if (next != null)
                return next.BuildUp(context, typeToBuild, existing, idToBuild);
            else
                return existing;
        }

        /// <summary>
        /// See <see cref="IBuilderStrategy.TearDown"/> for more information.
        /// </summary>
        public virtual object TearDown(IBuilderContext context,
                                       object item)
        {
            IBuilderStrategy next = context.GetNextInChain(this);

            if (next != null)
                return next.TearDown(context, item);
            else
                return item;
        }

        /// <summary>
        /// Creates a trace list of parameter types from a list of <see cref="IParameter"/> objects.
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The type list in string form</returns>
        protected static string ParametersToTypeList(params object[] parameters)
        {
            List<string> types = new List<string>();

            foreach (object parameter in parameters)
                types.Add(parameter.GetType().Name);

            return string.Join(", ", types.ToArray());
        }
    }
}