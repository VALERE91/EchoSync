using System;

namespace EchoSync.Replication
{
    public interface ILinkingContext
    {
        public int RegisterNetClass<T>(Func<NetObject> factory) where T : NetObject;
        
        public NetObject CreateNetObject(int classId);
    }
}