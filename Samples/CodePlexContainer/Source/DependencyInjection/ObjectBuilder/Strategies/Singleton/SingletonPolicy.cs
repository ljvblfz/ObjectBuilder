namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class SingletonPolicy : ISingletonPolicy
    {
        // Fields

        bool isSingleton;

        // Lifetime

        public SingletonPolicy(bool isSingleton)
        {
            this.isSingleton = isSingleton;
        }

        // Properties

        public bool IsSingleton
        {
            get { return isSingleton; }
        }
    }
}