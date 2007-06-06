using System.Collections.Generic;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IInterceptionPolicy : IBuilderPolicy, IEnumerable<KeyValuePair<MethodBase, List<ICallHandler>>>
    {
        // Properties

        InterceptionType InterceptionType { get; }
    }
}