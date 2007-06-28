using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IInterceptionPolicy : IBuilderPolicy, IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>>
    {
        int Count { get; }
        InterceptionType InterceptionType { get; }
        IList<IInterceptionHandler> this[MethodBase method] { get; }
        IEnumerable<MethodBase> Methods { get; }
    }
}