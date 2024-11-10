namespace EchoSync.Messages
{
    public class RpcMessage
    {
        public MessageTypes Type { get; set; } = MessageTypes.Rpc;

        public uint ObjectId { get; set; }

        public uint MethodId { get; set; }
        
        public byte[] Params { get; set; }
    }
}