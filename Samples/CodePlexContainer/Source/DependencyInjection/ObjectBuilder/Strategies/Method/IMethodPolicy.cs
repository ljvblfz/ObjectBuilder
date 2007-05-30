using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    /// <summary>
    /// Represents a policy for <see cref="MethodExecutionStrategy"/>.
    /// </summary>
    public interface IMethodPolicy : IBuilderPolicy
    {
        /// <summary>
        /// A collection of methods to be called on the object instance.
        /// </summary>
        Dictionary<string, IMethodCallInfo> Methods { get; }
    }
}