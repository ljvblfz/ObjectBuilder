namespace ObjectBuilder
{
    public interface IPropertySetterInfo
    {
        void Set(IBuilderContext context,
                 object instance,
                 object buildKey);
    }
}