using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class MethodPolicy : IMethodPolicy
    {
        readonly Dictionary<string, IMethodCallInfo> methods = new Dictionary<string, IMethodCallInfo>();

        public Dictionary<string, IMethodCallInfo> Methods
        {
            get { return methods; }
        }
    }
}