using System;

namespace EchoSync.Transport
{
    public interface IClient
    {
        public IPacketReceiver Receiver { get; protected set; }
        
        public IPacketSender Sender { get; protected set; }

        public Guid Identifier { get; protected set; }
    }
}