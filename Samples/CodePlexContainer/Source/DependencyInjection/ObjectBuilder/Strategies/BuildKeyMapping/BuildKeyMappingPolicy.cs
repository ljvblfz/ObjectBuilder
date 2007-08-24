namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class BuildKeyMappingPolicy : IBuildKeyMappingPolicy
    {
        readonly object newBuildKey;

        public BuildKeyMappingPolicy(object newBuildKey)
        {
            this.newBuildKey = newBuildKey;
        }

        public object Map(object buildKey)
        {
            return newBuildKey;
        }
    }
}