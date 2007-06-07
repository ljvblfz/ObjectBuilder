using CodePlex.DependencyInjection.ObjectBuilder;

namespace CodePlex.DependencyInjection
{
    class RecordingHandler : IInterceptionHandler
    {
        // Fields

        string message;

        // Lifetime

        public RecordingHandler()
        {
            message = "";
        }

        public RecordingHandler(string message)
        {
            this.message = string.Format(" ({0})", message);
        }

        // Method

        public IMethodReturn Invoke(IMethodInvocation call,
                                    GetNextHandlerDelegate getNext)
        {
            Recorder.Records.Add("Before Method" + message);
            IMethodReturn result = getNext().Invoke(call, getNext);
            Recorder.Records.Add("After Method" + message);
            return result;
        }
    }
}
