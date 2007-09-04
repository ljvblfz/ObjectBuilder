using System;

namespace Ottawa
{
    public interface IOttawaContainer : IDisposable
    {
        object this[Type service] { get; }

        object this[string key] { get; }

        //IOttawaContainer Parent { get; set; }

        //void AddChildContainer(IOttawaContainer childContainer);

        void AddComponent<TClass>(string key);

        void AddComponent(string key,
                          Type classType);

        void AddComponent<TService, TClass>(string key);

        void AddComponent(string key,
                          Type serviceType,
                          Type classType);

        void AddComponentWithLifestyle<TClass>(string key,
                                               LifestyleType lifestyle);

        void AddComponentWithLifestyle(string key,
                                       Type classType,
                                       LifestyleType lifestyle);

        void AddComponentWithLifestyle<TService, TClass>(string key,
                                                         LifestyleType lifestyle);

        void AddComponentWithLifestyle(string key,
                                       Type serviceType,
                                       Type classType,
                                       LifestyleType lifestyle);

        //void AddComponentWithProperties(string key,
        //                                Type classType,
        //                                IDictionary extendedProperties);

        //void AddComponentWithProperties(string key,
        //                                Type serviceType,
        //                                Type classType,
        //                                IDictionary extendedProperties);

        // Don't have IFacility, because we don't have IKernel

        //void AddFacility(string key,
        //                 IFacility facility);

        void Release(object instance);

        //void RemoveChildContainer(IOttawaContainer childContainer);

        T Resolve<T>();

        T Resolve<T>(string key);

        object Resolve(string key);

        object Resolve(Type service);

        object Resolve(string key,
                       Type service);
    }
}