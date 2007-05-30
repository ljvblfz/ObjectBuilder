using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IBuilderStrategy
    {
        // Methods

        object BuildUp(IBuilderContext context,
                       Type typeToBuild,
                       object existing,
                       string idToBuild);

        object TearDown(IBuilderContext context,
                        object item);
    }
}