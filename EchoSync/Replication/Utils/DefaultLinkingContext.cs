using System;
using System.Collections.Generic;
using System.Linq;
using EchoSync.Utils;

namespace EchoSync.Replication.Utils
{
    public class DefaultLinkingContext : ILinkingContext
    {
        private readonly Dictionary<int, Func<uint, NetObject>> _typeFactoryMap = new Dictionary<int, Func<uint, NetObject>>();

        public int RegisterNetClass<T>(Func<uint, NetObject> factory) where T : NetObject
        {
            int classId = TypeIdGenerator.GetOrRegisterTypeId(typeof(T));
            _typeFactoryMap.TryAdd(classId, factory);
            return classId;
        }
        
        public int RegisterNetClass(Type type, Func<uint, NetObject> factory)
        {
            int classId = TypeIdGenerator.GetOrRegisterTypeId(type);
            _typeFactoryMap.TryAdd(classId, factory);
            return classId;
        }

        public NetObject CreateNetObject(int classId, uint objectId)
        {
            if(_typeFactoryMap.TryGetValue(classId, out var factory))
            {
                return factory(objectId);
            }
            
            throw new InvalidOperationException($"NetObject with classId {classId} is not registered");
        }
    }
}