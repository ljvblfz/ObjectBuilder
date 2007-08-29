using System.Reflection;

namespace ObjectBuilder
{
    class OutputParameterCollection : ParameterCollection
    {
        public OutputParameterCollection(object[] arguments,
                                         ParameterInfo[] parameters)
            : base(arguments,
                   parameters,
                   delegate(ParameterInfo info)
                   {
                       return info.IsOut;
                   }) {}
    }
}