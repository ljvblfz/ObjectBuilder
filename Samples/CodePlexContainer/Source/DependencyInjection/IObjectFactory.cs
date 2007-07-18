using System;

namespace CodePlex.DependencyInjection
{
    public interface IObjectFactory
    {
        object Get(Type typeToBuild);
        TToBuild Get<TToBuild>();
        object Inject(object @object);
    }
}