using System;

namespace CodePlex.DependencyInjection.ObjectBuilder
{
    public interface IPolicyList
    {
        void Clear<TPolicyInterface>(object buildKey);

        void Clear(Type policyInterface,
                   object buildKey);

        void ClearAll();

        void ClearDefault<TPolicyInterface>();

        void ClearDefault(Type policyInterface);

        TPolicyInterface Get<TPolicyInterface>(object buildKey)
            where TPolicyInterface : IBuilderPolicy;

        IBuilderPolicy Get(Type policyInterface,
                           object buildKey);

        TPolicyInterface Get<TPolicyInterface>(object buildKey,
                                               bool localOnly)
            where TPolicyInterface : IBuilderPolicy;

        IBuilderPolicy Get(Type policyInterface,
                           object buildKey,
                           bool localOnly);

        TPolicyInterface GetNoDefault<TPolicyInterface>(object buildKey,
                                                        bool localOnly)
            where TPolicyInterface : IBuilderPolicy;

        IBuilderPolicy GetNoDefault(Type policyInterface,
                                    object buildKey,
                                    bool localOnly);

        void Set<TPolicyInterface>(TPolicyInterface policy,
                                   object buildKey)
            where TPolicyInterface : IBuilderPolicy;

        void Set(Type policyInterface,
                 IBuilderPolicy policy,
                 object buildKey);

        void SetDefault<TPolicyInterface>(TPolicyInterface policy)
            where TPolicyInterface : IBuilderPolicy;

        void SetDefault(Type policyInterface,
                        IBuilderPolicy policy);
    }
}