using System;

namespace ObjectBuilder
{
    public class StubObjectFactory : IObjectFactory
    {
        public object Get(Type typeToBuild)
        {
            return Activator.CreateInstance(typeToBuild);
        }

        public TToBuild Get<TToBuild>()
        {
            return (TToBuild)Get(typeof(TToBuild));
        }

        public object Inject(object @object)
        {
            throw new NotImplementedException();
        }
    }
}