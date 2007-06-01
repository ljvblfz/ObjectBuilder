namespace CodePlex.DependencyInjection.ObjectBuilder
{
    interface IPolicyList
    {
        IBuilderPolicy GetForKey(BuilderPolicyKey key);
    }
}