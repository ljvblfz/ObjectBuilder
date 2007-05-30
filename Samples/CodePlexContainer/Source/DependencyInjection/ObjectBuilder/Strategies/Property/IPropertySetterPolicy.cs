using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    /// <summary>
    /// Represents a policy for <see cref="PropertySetterStrategy"/>. The properties are
    /// indexed by the name of the property.
    /// </summary>
    public interface IPropertySetterPolicy : IBuilderPolicy
    {
        /// <summary>
        /// The property values to be set.
        /// </summary>
        Dictionary<string, IPropertySetterInfo> Properties { get; }
    }
}