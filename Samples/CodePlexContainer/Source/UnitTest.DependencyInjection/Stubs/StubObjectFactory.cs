using System;

namespace CodePlex.DependencyInjection
{
    class StubObjectFactory : IObjectFactory
    {
        public object Get(Type typeToBuild)
        {
            return Activator.CreateInstance(typeToBuild);
        }

        public TToBuild Get<TToBuild>()
        {
            return (TToBuild)Get(typeof(TToBuild));
        }
    }
}