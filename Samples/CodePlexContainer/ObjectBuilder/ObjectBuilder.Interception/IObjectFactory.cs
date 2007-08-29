using System;

namespace ObjectBuilder
{
    public interface IObjectFactory
    {
        object Get(Type typeToBuild);
        TToBuild Get<TToBuild>();
        object Inject(object @object);
    }
}