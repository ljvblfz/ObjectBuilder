using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
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