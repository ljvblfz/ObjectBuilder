using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public struct BuilderPolicyKey
    {
        // Fields

        Type PolicyType;
        Type BuildType;
        string BuildID;

        // Lifetime

        public BuilderPolicyKey(Type policyType,
                                Type typePolicyAppliesTo,
                                string idPolicyAppliesTo)
        {
            PolicyType = policyType;
            BuildType = typePolicyAppliesTo;
            BuildID = idPolicyAppliesTo;
        }
    }
}