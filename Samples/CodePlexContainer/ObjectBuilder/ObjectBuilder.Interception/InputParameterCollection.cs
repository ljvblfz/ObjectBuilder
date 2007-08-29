using System.Reflection;

namespace ObjectBuilder
{
    class InputParameterCollection : ParameterCollection
    {
        public InputParameterCollection(object[] arguments,
                                        ParameterInfo[] parameters)
            : base(arguments,
                   parameters,
                   delegate(ParameterInfo info)
                   {
                       return !info.IsOut;
                   }) {}
    }
}