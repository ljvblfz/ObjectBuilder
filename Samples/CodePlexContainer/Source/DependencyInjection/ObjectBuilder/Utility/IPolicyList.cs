namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IPolicyList
    {
        IBuilderPolicy GetForKey(BuilderPolicyKey key,
                                 bool localOnly);
    }
}