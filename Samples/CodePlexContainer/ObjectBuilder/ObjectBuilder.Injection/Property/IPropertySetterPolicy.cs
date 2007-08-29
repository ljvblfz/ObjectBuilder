using System.Collections.Generic;

namespace ObjectBuilder
{
    public interface IPropertySetterPolicy : IBuilderPolicy
    {
        List<IPropertySetterInfo> Properties { get; }
    }
}