using System;

namespace CodePlex.DependencyInjection
{
    public interface IMethodReturn
    {
        IParameterCollection Outputs { get; }
        object ReturnValue { get; set; }
        Exception Exception { get; set; }
    }
}