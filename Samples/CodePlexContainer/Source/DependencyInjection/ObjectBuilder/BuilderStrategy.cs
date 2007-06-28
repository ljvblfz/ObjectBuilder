using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public abstract class BuilderStrategy : IBuilderStrategy
    {
        public TItem BuildUp<TItem>(IBuilderContext context,
                                    TItem existing,
                                    string idToBuild)
        {
            return (TItem)BuildUp(context, typeof(TItem), existing, idToBuild);
        }

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

        protected static string ParametersToTypeList(params object[] parameters)
        {
            List<string> types = new List<string>();

            foreach (object parameter in parameters)
                types.Add(parameter.GetType().Name);

            return string.Join(", ", types.ToArray());
        }

        public virtual object TearDown(IBuilderContext context,
                                       object item)
        {
            IBuilderStrategy next = context.GetNextInChain(this);

            if (next != null)
                return next.TearDown(context, item);
            else
                return item;
        }
    }
}