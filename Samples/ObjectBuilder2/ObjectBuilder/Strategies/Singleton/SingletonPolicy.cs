namespace ObjectBuilder
{
    public class SingletonPolicy : ISingletonPolicy
    {
        readonly bool isSingleton;

        public SingletonPolicy(bool isSingleton)
        {
            this.isSingleton = isSingleton;
        }

        public bool IsSingleton
        {
            get { return isSingleton; }
        }
    }
}