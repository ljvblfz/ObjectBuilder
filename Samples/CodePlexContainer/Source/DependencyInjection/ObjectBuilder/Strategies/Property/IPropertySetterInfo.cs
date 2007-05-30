using System;
using System.Reflection;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IPropertySetterInfo
    {
        // Methods

        object GetValue(IBuilderContext context,
                        Type type,
                        string id,
                        PropertyInfo propInfo);

        PropertyInfo SelectProperty(IBuilderContext context,
                                    Type type,
                                    string id);
    }
}