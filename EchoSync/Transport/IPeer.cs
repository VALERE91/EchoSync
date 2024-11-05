using System;

namespace EchoSync.Transport
{
    public interface IPeer : IDisposable
    {
        public IPacketReceiver Receiver { get; }
        
        public IPacketSender Sender { get; }

        public Guid Identifier { get; }
    }
}