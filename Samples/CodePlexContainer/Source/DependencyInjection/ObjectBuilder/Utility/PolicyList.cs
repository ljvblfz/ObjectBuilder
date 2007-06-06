using System;
using System.Collections.Generic;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public class PolicyList : IPolicyList
    {
        // Fields

        IPolicyList innerPolicyList;
        object lockObject = new object();
        Dictionary<BuilderPolicyKey, IBuilderPolicy> policies = new Dictionary<BuilderPolicyKey, IBuilderPolicy>();

        // Lifetime

        public PolicyList()
            : this(null) {}

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
            return Get(policyInterface, typePolicyAppliesTo, idPolicyAppliesTo, false);
        }

        IBuilderPolicy Get(Type policyInterface,
                           Type typePolicyAppliesTo,
                           string idPolicyAppliesTo,
                           bool localOnly)
        {
            BuilderPolicyKey key = new BuilderPolicyKey(policyInterface, typePolicyAppliesTo, idPolicyAppliesTo ?? "");
            BuilderPolicyKey keyForDefaultType = new BuilderPolicyKey(policyInterface, typePolicyAppliesTo, null);
            BuilderPolicyKey keyForDefaultAllTypes = new BuilderPolicyKey(policyInterface, null, null);
            IPolicyList thisPolicyList = this;

            return
                thisPolicyList.GetForKey(key, localOnly) ??
                thisPolicyList.GetForKey(keyForDefaultType, localOnly) ??
                thisPolicyList.GetForKey(keyForDefaultAllTypes, localOnly);
        }

        IBuilderPolicy IPolicyList.GetForKey(BuilderPolicyKey key,
                                             bool localOnly)
        {
            lock (lockObject)
            {
                IBuilderPolicy policy;
                if (policies.TryGetValue(key, out policy))
                    return policy;
            }

            if (localOnly)
                return null;
            else
                return innerPolicyList.GetForKey(key, localOnly);
        }

        public TPolicy GetLocal<TPolicy>(Type typePolicyAppliesTo,
                                         string idPolicyAppliesTo)
            where TPolicy : IBuilderPolicy
        {
            return (TPolicy)GetLocal(typeof(TPolicy), typePolicyAppliesTo, idPolicyAppliesTo);
        }

        public IBuilderPolicy GetLocal(Type policyInterface,
                                       Type typePolicyAppliesTo,
                                       string idPolicyAppliesTo)
        {
            return Get(policyInterface, typePolicyAppliesTo, idPolicyAppliesTo, true);
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
            BuilderPolicyKey key = new BuilderPolicyKey(policyInterface, typePolicyAppliesTo, idPolicyAppliesTo ?? "");

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
            BuilderPolicyKey key = new BuilderPolicyKey(policyInterface, null, null);

            lock (lockObject)
                policies[key] = policy;
        }

        public void SetDefaultForType<TPolicyInterface>(TPolicyInterface policy,
                                                        Type typePolicyAppliesTo)
            where TPolicyInterface : IBuilderPolicy
        {
            SetDefaultForType(typeof(TPolicyInterface), policy, typePolicyAppliesTo);
        }

        public void SetDefaultForType(Type policyInterface,
                                      IBuilderPolicy policy,
                                      Type typePolicyAppliesTo)
        {
            BuilderPolicyKey key = new BuilderPolicyKey(policyInterface, typePolicyAppliesTo, null);

            lock (lockObject)
                policies[key] = policy;
        }

        // Inner types

        class NullPolicyList : IPolicyList
        {
            public IBuilderPolicy GetForKey(BuilderPolicyKey key,
                                            bool localOnly)
            {
                return null;
            }
        }
    }
}