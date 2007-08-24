using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IPropertySetterPolicy : IBuilderPolicy
    {
        List<IPropertySetterInfo> Properties { get; }
    }
}