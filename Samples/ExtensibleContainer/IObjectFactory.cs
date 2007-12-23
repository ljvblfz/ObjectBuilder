using System;

namespace ObjectBuilder
{
    public interface IObjectFactory
    {
        object Get(Type type);
        object Get(string name);

        T Get<T>();
        T Get<T>(string name);

        object Inject(object @object);
    }
}