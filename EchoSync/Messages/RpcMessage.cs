using System;
using EchoSync.Serialization;

namespace EchoSync.Messages
{
    public class RpcMessage
    {
        public MessageTypes Type { get; set; } = MessageTypes.Rpc;

        public uint ObjectId { get; set; }

        public uint MethodId { get; set; }
        
        public byte[]? Params { get; set; }
        
        public void Serialize(ref BitStream bitStream)
        {
            var writer = new EchoBitStream();
            
            writer.Write(ref bitStream, (byte)Type);
            writer.Write<uint>(ref bitStream, ObjectId);
            writer.Write<uint>(ref bitStream, MethodId);
        }

        public Span<byte> Deserialize(ref Span<byte> receivedData)
        {
            var bitStream = new BitStream(receivedData);
            var reader = new EchoBitStream();
            
            Type = (MessageTypes)reader.Read<byte>(ref bitStream);
            ObjectId = reader.Read<uint>(ref bitStream);
            MethodId = reader.Read<uint>(ref bitStream);

            return receivedData.Slice(bitStream.BytePosition);
        }
    }
}