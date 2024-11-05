using System;

namespace EchoSync.Transport
{
    public interface IClient : IDisposable, ITickable
    {
        public IPacketReceiver Receiver { get; protected set; }
        
        public IPacketSender Sender { get; protected set; }

        public Guid Identifier { get; protected set; }

        public void Connect();
    }
}