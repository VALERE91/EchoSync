using System;
using System.Collections.Generic;
using EchoSync.Utils;

namespace EchoSync.Replication
{
    public class DefaultLinkingContext : ILinkingContext
    {
        private readonly Dictionary<int, Func<NetObject>> _typeFactoryMap = new Dictionary<int, Func<NetObject>>();
        
        public int RegisterNetClass<T>(Func<NetObject> factory) where T : NetObject
        {
            int classId = TypeIdGenerator.GetOrRegisterTypeId(typeof(T));
            _typeFactoryMap.TryAdd(classId, factory);
            return classId;
        }

        public NetObject CreateNetObject(int classId)
        {
            if(_typeFactoryMap.TryGetValue(classId, out var factory))
            {
                return factory();
            }
            
            throw new InvalidOperationException($"NetObject with classId {classId} is not registered");
        }
    }
}