using System.Collections.Generic;
using System.Reflection;
using CodePlex.DependencyInjection.ObjectBuilder;

namespace CodePlex.DependencyInjection
{
    public interface IInterceptionPolicy : IBuilderPolicy, IEnumerable<KeyValuePair<MethodBase, List<ICallHandler>>>
    {
        // Properties

        InterceptionType InterceptionType { get; }
    }
}