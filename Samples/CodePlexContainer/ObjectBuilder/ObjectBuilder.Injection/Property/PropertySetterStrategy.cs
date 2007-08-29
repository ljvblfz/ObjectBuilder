namespace ObjectBuilder
{
    public class PropertySetterStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            IPropertySetterPolicy policy = context.Policies.Get<IPropertySetterPolicy>(buildKey);

            if (existing != null && policy != null)
                foreach (IPropertySetterInfo property in policy.Properties)
                    property.Set(context, existing, buildKey);

            return base.BuildUp(context, buildKey, existing);
        }
    }
}