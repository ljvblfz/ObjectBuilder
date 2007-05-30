using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class PolicyList
    {
        // Fields

        Dictionary<BuilderPolicyKey, IBuilderPolicy> policies = new Dictionary<BuilderPolicyKey, IBuilderPolicy>();
        object lockObject = new object();

        // Lifetime

        public PolicyList(params PolicyList[] policiesToCopy)
        {
            if (policiesToCopy != null)
                foreach (PolicyList policyList in policiesToCopy)
                    AddPolicies(policyList);
        }

        // Properties

        public int Count
        {
            get
            {
                lock (lockObject)
                    return policies.Count;
            }
        }

        // Methods

        public void AddPolicies(PolicyList policiesToCopy)
        {
            lock (lockObject)
                if (policiesToCopy != null)
                    foreach (KeyValuePair<BuilderPolicyKey, IBuilderPolicy> kvp in policiesToCopy.policies)
                        policies[kvp.Key] = kvp.Value;
        }

        public void Clear<TPolicyInterface>(Type typePolicyAppliesTo,
                                            string idPolicyAppliesTo)
        {
            Clear(typeof(TPolicyInterface), typePolicyAppliesTo, idPolicyAppliesTo);
        }

        public void Clear(Type policyInterface,
                          Type typePolicyAppliesTo,
                          string idPolicyAppliesTo)
        {
            lock (lockObject)
                policies.Remove(new BuilderPolicyKey(policyInterface, typePolicyAppliesTo, idPolicyAppliesTo));
        }

        public void ClearAll()
        {
            lock (lockObject)
                policies.Clear();
        }

        public void ClearDefault<TPolicyInterface>()
        {
            ClearDefault(typeof(TPolicyInterface));
        }

        public void ClearDefault(Type policyInterface)
        {
            Clear(policyInterface, null, null);
        }

        public TPolicyInterface Get<TPolicyInterface>(Type typePolicyAppliesTo,
                                                      string idPolicyAppliesTo)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)Get(typeof(TPolicyInterface), typePolicyAppliesTo, idPolicyAppliesTo);
        }

        public IBuilderPolicy Get(Type policyInterface,
                                  Type typePolicyAppliesTo,
                                  string idPolicyAppliesTo)
        {
            BuilderPolicyKey key = new BuilderPolicyKey(policyInterface, typePolicyAppliesTo, idPolicyAppliesTo);

            lock (lockObject)
            {
                IBuilderPolicy policy;

                if (policies.TryGetValue(key, out policy))
                    return policy;

                BuilderPolicyKey defaultKey = new BuilderPolicyKey(policyInterface, null, null);
                if (policies.TryGetValue(defaultKey, out policy))
                    return policy;

                return null;
            }
        }

        public void Set<TPolicyInterface>(TPolicyInterface policy,
                                          Type typePolicyAppliesTo,
                                          string idPolicyAppliesTo)
            where TPolicyInterface : IBuilderPolicy
        {
            Set(typeof(TPolicyInterface), policy, typePolicyAppliesTo, idPolicyAppliesTo);
        }

        public void Set(Type policyInterface,
                        IBuilderPolicy policy,
                        Type typePolicyAppliesTo,
                        string idPolicyAppliesTo)
        {
            BuilderPolicyKey key = new BuilderPolicyKey(policyInterface, typePolicyAppliesTo, idPolicyAppliesTo);

            lock (lockObject)
                policies[key] = policy;
        }

        public void SetDefault<TPolicyInterface>(TPolicyInterface policy)
            where TPolicyInterface : IBuilderPolicy
        {
            SetDefault(typeof(TPolicyInterface), policy);
        }

        public void SetDefault(Type policyInterface,
                               IBuilderPolicy policy)
        {
            Set(policyInterface, policy, null, null);
        }
    }
}