namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public delegate IMethodReturn InvokeHandlerDelegate(IMethodInvocation call,
                                                        GetNextHandlerDelegate getNext);

    public delegate InvokeHandlerDelegate GetNextHandlerDelegate();

    public interface ICallHandler
    {
        IMethodReturn Invoke(IMethodInvocation call,
                             GetNextHandlerDelegate getNext);
    }
}