using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EchoSync.RPC;
using EchoSync.Serialization;
using EchoSync.Transport;
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

        public virtual void InvokeRpc(uint methodId, Span<byte> parameters)
        {
            
        }
    }
    
    public abstract class NetObject<T> : NetObject where T : NetObject<T>
    {
        public static NetSchema? NetSchema { get; private set; }

        private static Dictionary<string, int> RpcNameToIndex { get; } = new Dictionary<string, int>();
        
        public static List<RpcMethod>? RpcMethods { get; private set; }
        
        protected NetObject(Func<uint, NetObject<T>> factory)
        {
            var linkingContext = ServiceLocator.Get<ILinkingContext>();
            ClassId = linkingContext.RegisterNetClass<T>(factory);
            
            var objectIdGenerator = ServiceLocator.Get<IObjectIdGenerator>();
            ObjectId = HasAuthority() ? 
                objectIdGenerator.GenerateServerId() : 
                objectIdGenerator.GenerateClientId();

            NetSchema ??= NetSchemaBuilder.CreateSchema<T>();
            
            RpcMethods ??= RpcMethodInfoBuilder.CreateRpcMethodInfo<T>();
            if (RpcMethods?.Count == RpcNameToIndex.Count) return;
            RpcNameToIndex.Clear();
            for (int i = 0; i < RpcMethods?.Count; i++)
            {
                RpcNameToIndex[RpcMethods[i].MethodeName] = i;
            }
        }

        protected NetObject(uint objectId)
        {
            ObjectId = objectId;
            NetSchema ??= NetSchemaBuilder.CreateSchema<T>();
            
            RpcMethods ??= RpcMethodInfoBuilder.CreateRpcMethodInfo<T>();
            if (RpcMethods?.Count == RpcNameToIndex.Count) return;
            RpcNameToIndex.Clear();
            for (int i = 0; i < RpcMethods?.Count; i++)
            {
                RpcNameToIndex[RpcMethods[i].MethodeName] = i;
            }
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

        public override void InvokeRpc(uint methodId, Span<byte> parameters)
        {
            if(RpcMethods == null)
            {
                throw new Exception("Trying to call a RPC method on an object that has no RPC methods");
            }
            
            var rpcMethod = RpcMethods[(int)methodId];
            rpcMethod?.Invoke(this, parameters);
        }
        
        public void CallRpc(string method, params object[] parameters)
        {
            //Get the RPC info from the name
            if (!RpcNameToIndex.TryGetValue(method, out int methodId))
            {
                throw new Exception($"Method {method} not found");
            }
            
            //Get the RPC method
            if (RpcMethods == null)
            {
                throw new Exception("Trying to call a RPC method on an object that has no RPC methods");
            }
            
            var rpcMethod = RpcMethods[methodId];
            if (rpcMethod == null)
            {
                throw new Exception($"Method {method} not found");
            }
            
            //Create the RpcMessage
            var rpcMessage = new Messages.RpcMessage
            {
                ObjectId = ObjectId,
                MethodId = (uint)methodId
            };
            
            //Serialize the message
            Span<byte> messageBuffer = stackalloc byte[1024];
            var bitStream = new BitStream(messageBuffer);
            rpcMessage.Serialize(ref bitStream);
            rpcMethod.SerializeParameters(ref bitStream, parameters);
            
            //Send it to the server
            ReplicationEngine.GetLocalPeer().Sender.SendPacket(0, Reliability.Reliable, 
                messageBuffer.Slice(0, bitStream.BytePosition));
        }
    }
}