using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IInterceptionPolicy : IBuilderPolicy, IEnumerable<KeyValuePair<MethodBase, List<IInterceptionHandler>>>
    {
        // Properties

        IList<IInterceptionHandler> this[MethodBase method] { get; }
        int Count { get; }
        InterceptionType InterceptionType { get; }
        IEnumerable<MethodBase> Methods { get; }
    }
}