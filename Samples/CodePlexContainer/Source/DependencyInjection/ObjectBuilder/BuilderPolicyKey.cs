using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public struct BuilderPolicyKey
    {
        // Fields

        public readonly string BuildID;
        public readonly Type BuildType;
        public readonly Type PolicyType;

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