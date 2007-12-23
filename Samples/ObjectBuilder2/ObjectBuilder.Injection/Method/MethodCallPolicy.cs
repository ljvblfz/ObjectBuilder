using System.Collections.Generic;

namespace ObjectBuilder
{
    public class MethodCallPolicy : IMethodCallPolicy
    {
        readonly List<IMethodCallInfo> methods = new List<IMethodCallInfo>();

        public List<IMethodCallInfo> Methods
        {
            get { return methods; }
        }
    }
}