using System;
using System.Collections.Generic;

namespace ObjectBuilder
{
    public class PolicyList : IPolicyList
    {
        readonly IPolicyList innerPolicyList;
        readonly object lockObject = new object();
        readonly Dictionary<PolicyKey, IBuilderPolicy> policies = new Dictionary<PolicyKey, IBuilderPolicy>();

        public PolicyList()
            : this(null) {}

        public PolicyList(IPolicyList innerPolicyList)
        {
            this.innerPolicyList = innerPolicyList ?? new NullPolicyList();
        }

        public int Count
        {
            get
            {
                lock (lockObject)
                    return policies.Count;
            }
        }

        public void Clear<TPolicyInterface>(object buildKey)
        {
            Clear(typeof(TPolicyInterface), buildKey);
        }

        public void Clear(Type policyInterface,
                          object buildKey)
        {
            lock (lockObject)
                policies.Remove(new PolicyKey(policyInterface, buildKey));
        }

        public void ClearAll()
        {
            lock (lockObject)
                policies.Clear();
        }

        public void ClearDefault<TPolicyInterface>()
        {
            Clear(typeof(TPolicyInterface), null);
        }

        public void ClearDefault(Type policyInterface)
        {
            Clear(policyInterface, null);
        }

        public TPolicyInterface Get<TPolicyInterface>(object buildKey)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)Get(typeof(TPolicyInterface), buildKey, false);
        }

        public IBuilderPolicy Get(Type policyInterface,
                                  object buildKey)
        {
            return Get(policyInterface, buildKey, false);
        }

        public TPolicyInterface Get<TPolicyInterface>(object buildKey,
                                                      bool localOnly)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)Get(typeof(TPolicyInterface), buildKey, localOnly);
        }

        public IBuilderPolicy Get(Type policyInterface,
                                  object buildKey,
                                  bool localOnly)
        {
            Type buildType;

            if (!BuilderStrategy.TryGetTypeFromBuildKey(buildKey, out buildType) || !buildType.IsGenericType)
                return
                    GetNoDefault(policyInterface, buildKey, localOnly) ??
                    GetNoDefault(policyInterface, null, localOnly);

            return
                GetNoDefault(policyInterface, buildKey, localOnly) ??
                GetNoDefault(policyInterface, buildType.GetGenericTypeDefinition(), localOnly) ??
                GetNoDefault(policyInterface, null, localOnly);
        }

        public TPolicyInterface GetNoDefault<TPolicyInterface>(object buildKey,
                                                               bool localOnly)
            where TPolicyInterface : IBuilderPolicy
        {
            return (TPolicyInterface)GetNoDefault(typeof(TPolicyInterface), buildKey, localOnly);
        }

        public IBuilderPolicy GetNoDefault(Type policyInterface,
                                           object buildKey,
                                           bool localOnly)
        {
            lock (lockObject)
            {
                IBuilderPolicy policy;
                if (policies.TryGetValue(new PolicyKey(policyInterface, buildKey), out policy))
                    return policy;
            }

            if (localOnly)
                return null;

            return innerPolicyList.GetNoDefault(policyInterface, buildKey, localOnly);
        }

        public void Set<TPolicyInterface>(TPolicyInterface policy,
                                          object buildKey)
            where TPolicyInterface : IBuilderPolicy
        {
            Set(typeof(TPolicyInterface), policy, buildKey);
        }

        public void Set(Type policyInterface,
                        IBuilderPolicy policy,
                        object buildKey)
        {
            lock (lockObject)
                policies[new PolicyKey(policyInterface, buildKey)] = policy;
        }

        public void SetDefault<TPolicyInterface>(TPolicyInterface policy)
            where TPolicyInterface : IBuilderPolicy
        {
            Set(typeof(TPolicyInterface), policy, null);
        }

        public void SetDefault(Type policyInterface,
                               IBuilderPolicy policy)
        {
            Set(policyInterface, policy, null);
        }

        class NullPolicyList : IPolicyList
        {
            public void Clear<TPolicyInterface>(object buildKey)
            {
                throw new NotImplementedException();
            }

            public void Clear(Type policyInterface,
                              object buildKey)
            {
                throw new NotImplementedException();
            }

            public void ClearAll()
            {
                throw new NotImplementedException();
            }

            public void ClearDefault<TPolicyInterface>()
            {
                throw new NotImplementedException();
            }

            public void ClearDefault(Type policyInterface)
            {
                throw new NotImplementedException();
            }

            public TPolicyInterface Get<TPolicyInterface>(object buildKey) where TPolicyInterface : IBuilderPolicy
            {
                return default(TPolicyInterface);
            }

            public IBuilderPolicy Get(Type policyInterface,
                                      object buildKey)
            {
                return null;
            }

            public TPolicyInterface Get<TPolicyInterface>(object buildKey,
                                                          bool localOnly)
                where TPolicyInterface : IBuilderPolicy
            {
                return default(TPolicyInterface);
            }

            public IBuilderPolicy Get(Type policyInterface,
                                      object buildKey,
                                      bool localOnly)
            {
                return null;
            }

            public TPolicyInterface GetNoDefault<TPolicyInterface>(object buildKey,
                                                                   bool localOnly)
                where TPolicyInterface : IBuilderPolicy
            {
                return default(TPolicyInterface);
            }

            public IBuilderPolicy GetNoDefault(Type policyInterface,
                                               object buildKey,
                                               bool localOnly)
            {
                return null;
            }

            public void Set<TPolicyInterface>(TPolicyInterface policy,
                                              object buildKey)
                where TPolicyInterface : IBuilderPolicy
            {
                throw new NotImplementedException();
            }

            public void Set(Type policyInterface,
                            IBuilderPolicy policy,
                            object buildKey)
            {
                throw new NotImplementedException();
            }

            public void SetDefault<TPolicyInterface>(TPolicyInterface policy)
                where TPolicyInterface : IBuilderPolicy
            {
                throw new NotImplementedException();
            }

            public void SetDefault(Type policyInterface,
                                   IBuilderPolicy policy)
            {
                throw new NotImplementedException();
            }
        }

        struct PolicyKey
        {
#pragma warning disable 219
            public readonly object BuildKey;
            public readonly Type PolicyType;
#pragma warning restore 219

            public PolicyKey(Type policyType,
                             object buildKey)
            {
                PolicyType = policyType;
                BuildKey = buildKey;
            }
        }
    }
}