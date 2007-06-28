using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IPropertySetterPolicy : IBuilderPolicy
    {
        Dictionary<string, IPropertySetterInfo> Properties { get; }
    }
}