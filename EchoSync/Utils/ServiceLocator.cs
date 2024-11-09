using System;
using System.Collections.Generic;

namespace EchoSync.Utils
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<int, object> _instanceMap = new Dictionary<int, object>();
        
        public static void Provide<T>(T service)
        {
            int typeId = TypeIdGenerator.GetOrRegisterTypeId(typeof(T));
            if (service != null && !_instanceMap.TryAdd(typeId, service))
            { 
                throw new InvalidOperationException($"Service of type {typeof(T).Name} is already provided");
            }
        }
        
        public static T Get<T>()
        {
            int typeId = TypeIdGenerator.GetOrRegisterTypeId(typeof(T));
            if (!_instanceMap.TryGetValue(typeId, out var service))
            {
                throw new InvalidOperationException($"Service of type {typeof(T).Name} is not provided");
            }

            return (T)service;
        }
    }
}