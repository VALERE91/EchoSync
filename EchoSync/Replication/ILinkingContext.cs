using System;

namespace EchoSync.Replication
{
    public interface ILinkingContext
    {
        public int RegisterNetClass<T>(Func<uint, NetObject> factory) where T : NetObject;
        
        public int NetClassId<T>() where T : NetObject;
        
        public NetObject CreateNetObject(int classId, uint objectId);
    }
}