namespace ObjectBuilder
{
    public class MethodCallStrategy : BuilderStrategy
    {
        public override object BuildUp(IBuilderContext context,
                                       object buildKey,
                                       object existing)
        {
            IMethodCallPolicy policy = context.Policies.Get<IMethodCallPolicy>(buildKey);

            if (existing != null && policy != null)
                foreach (IMethodCallInfo method in policy.Methods)
                    method.Execute(context, existing, buildKey);

            return base.BuildUp(context, buildKey, existing);
        }
    }
}