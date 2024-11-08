using System;
using System.Linq;
using EchoSync.Serialization;
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
        public static NetSchema? NetSchema { get; private set; }

        protected NetObject(Func<NetObject<T>> factory)
        {
            if (!HasAuthority())
            {
                throw new NotImplementedException("Predictive spawning is not supported yet");
            }
            
            var linkingContext = ServiceLocator.Get<ILinkingContext>();
            ClassId = linkingContext.RegisterNetClass<T>(factory);
            
            var objectIdGenerator = ServiceLocator.Get<IObjectIdGenerator>();
            ObjectId = HasAuthority() ? 
                objectIdGenerator.GenerateServerId() : 
                objectIdGenerator.GenerateClientId();

            NetSchema ??= NetSchemaBuilder.CreateSchema<T>();
        }

        public void NetWriteTo(BitStream bitStream)
        {
            if (NetSchema == null)
            {
                return;
            }
            
            foreach (NetPropertyInfo property in NetSchema.Properties)
            {
                var value = property.Getter(this);
                //Write the value
            }
        }
        
        public void NetReadFrom(ReadOnlyBitStream bitStream)
        {
            if (NetSchema == null)
            {
                return;
            }
            
            foreach (NetPropertyInfo property in NetSchema.Properties)
            {
                //Read the value
                var value = 0;
                property.Setter(this, value);
            }
        }
    }
}