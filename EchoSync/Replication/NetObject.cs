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

        public NetObject()
        {
            if (!HasAuthority())
            {
                return;
            }
            
            ReplicationEngine.RegisterNetObject(this);
        }
        
        protected bool HasAuthority()
        {
            return ReplicationEngine.HasAuthority();
        }

        public virtual void NetWriteTo(IBitWriter bitStreamWriter, ref BitStream bitStream)
        {
            bitStreamWriter.Write(ref bitStream, ClassId);
            bitStreamWriter.Write(ref bitStream, ObjectId);
        }

        public virtual void NetReadFrom(IBitReader bitStreamReader, ref BitStream bitStream)
        {
            //Those are read by the replication engine
        }
    }
    
    public abstract class NetObject<T> : NetObject where T : NetObject<T>
    {
        public static NetSchema? NetSchema { get; private set; }

        protected NetObject(Func<uint, NetObject<T>> factory)
        {
            var linkingContext = ServiceLocator.Get<ILinkingContext>();
            ClassId = linkingContext.RegisterNetClass<T>(factory);
            
            var objectIdGenerator = ServiceLocator.Get<IObjectIdGenerator>();
            ObjectId = HasAuthority() ? 
                objectIdGenerator.GenerateServerId() : 
                objectIdGenerator.GenerateClientId();

            NetSchema ??= NetSchemaBuilder.CreateSchema<T>();
        }

        protected NetObject(uint objectId)
        {
            ObjectId = objectId;
            NetSchema ??= NetSchemaBuilder.CreateSchema<T>();
        }
        
        public override void NetWriteTo(IBitWriter bitStreamWriter, ref BitStream bitStream)
        {
            base.NetWriteTo(bitStreamWriter, ref bitStream);
            if (NetSchema == null)
            {
                return;
            }
            
            foreach (NetPropertyInfo property in NetSchema.Properties)
            {
                var value = property.Getter(this);
                bitStreamWriter.Write(property.Property.PropertyType, ref bitStream, value);
            }
        }
        
        public override void NetReadFrom(IBitReader bitStreamReader, ref BitStream bitStream)
        {
            base.NetReadFrom(bitStreamReader, ref bitStream);
            if (NetSchema == null)
            {
                return;
            }
            
            foreach (NetPropertyInfo property in NetSchema.Properties)
            {
                //Read the value
                var value = bitStreamReader.Read(property.Property.PropertyType, ref bitStream);
                property.Setter(this, value);
            }
        }
    }
}