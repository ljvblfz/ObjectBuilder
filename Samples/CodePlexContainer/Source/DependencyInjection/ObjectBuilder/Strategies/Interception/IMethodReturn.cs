using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IMethodReturn
    {
        IParameterCollection Outputs { get; }
        object ReturnValue { get; set; }
        Exception Exception { get; set; }
    }
}