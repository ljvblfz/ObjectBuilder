using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    interface IPolicyList
    {
        IBuilderPolicy GetDefault(BuilderPolicyKey key);
        IBuilderPolicy GetWithoutDefault(BuilderPolicyKey key);
    }

    public class PolicyList : IPolicyList
    {
        // Fields

        Dictionary<BuilderPolicyKey, IBuilderPolicy> policies = new Dictionary<BuilderPolicyKey, IBuilderPolicy>();
        object lockObject = new object();
        IPolicyList innerPolicyList;

        // Lifetime

        public PolicyList()
        {
            innerPolicyList = new NullPolicyList();
        }

        public PolicyList(PolicyList innerPolicyList)
        {
            this.innerPolicyList = (IPolicyList)innerPolicyList ?? new NullPolicyList();
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
            BuilderPolicyKey defaultKey = new BuilderPolicyKey(policyInterface, null, null);

            return
                ((IPolicyList)this).GetWithoutDefault(key) ??
                ((IPolicyList)this).GetDefault(defaultKey);
        }

        IBuilderPolicy IPolicyList.GetDefault(BuilderPolicyKey key)
        {
            lock (lockObject)
            {
                IBuilderPolicy policy;
                if (policies.TryGetValue(key, out policy))
                    return policy;
            }

            return innerPolicyList.GetDefault(key);
        }

        IBuilderPolicy IPolicyList.GetWithoutDefault(BuilderPolicyKey key)
        {
            lock (lockObject)
            {
                IBuilderPolicy policy;

                if (policies.TryGetValue(key, out policy))
                    return policy;
            }

            return innerPolicyList.GetWithoutDefault(key);
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

        // Inner types

        class NullPolicyList : IPolicyList
        {
            public IBuilderPolicy GetDefault(BuilderPolicyKey key)
            {
                return null;
            }

            public IBuilderPolicy GetWithoutDefault(BuilderPolicyKey key)
            {
                return null;
            }
        }
    }
}