namespace ObjectBuilder
{
    public delegate IMethodReturn InvokeHandlerDelegate(IMethodInvocation call,
                                                        GetNextHandlerDelegate getNext);

    public delegate InvokeHandlerDelegate GetNextHandlerDelegate();

    public interface IInterceptionHandler
    {
        IMethodReturn Invoke(IMethodInvocation call,
                             GetNextHandlerDelegate getNext);
    }
}