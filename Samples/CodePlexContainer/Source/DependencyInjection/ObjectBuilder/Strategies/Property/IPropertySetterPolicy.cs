using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IPropertySetterPolicy : IBuilderPolicy
    {
        // Properties

        Dictionary<string, IPropertySetterInfo> Properties { get; }
    }
}