using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class PropertySetterPolicy : IPropertySetterPolicy
    {
        // Fields

        Dictionary<string, IPropertySetterInfo> properties = new Dictionary<string, IPropertySetterInfo>();

        // Properties

        public Dictionary<string, IPropertySetterInfo> Properties
        {
            get { return properties; }
        }
    }
}