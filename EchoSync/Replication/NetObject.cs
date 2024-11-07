using System;
using EchoSync.Utils;

namespace EchoSync.Replication
{
    public abstract class NetObject
    {
        protected IReplicationEngine ReplicationEngine { get; } = ServiceLocator.Get<IReplicationEngine>();
        
        public int ClassId { get; protected set; }

        public uint ObjectId { get; protected set; }

        protected bool HasAuthority()
        {
            return ReplicationEngine.HasAuthority();
        }
    }
    
    public abstract class NetObject<T> : NetObject where T : NetObject<T>
    {
        protected NetObject(Func<NetObject<T>> factory)
        {
            var linkingContext = ServiceLocator.Get<ILinkingContext>();
            ClassId = linkingContext.RegisterNetClass<T>(factory);
            
            var objectIdGenerator = ServiceLocator.Get<IObjectIdGenerator>();
            ObjectId = HasAuthority() ? 
                objectIdGenerator.GenerateServerId() : 
                objectIdGenerator.GenerateClientId();
        }
    }
}