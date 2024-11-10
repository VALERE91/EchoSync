using System;
using EchoSync.Transport;
using LiteNetLib;
using LiteNetLibAdapters.Receivers;
using LiteNetLibAdapters.Senders;

namespace LiteNetLibAdapters
{
    public class LiteNetLibPeer : IPeer
    {
        private readonly PeerReceiver _peerReceiver = new PeerReceiver();
        private readonly NetPeer _peer;

        public LiteNetLibPeer(NetPeer peer)
        {
            _peer = peer;
            Sender = new PeerSender(peer);
        }

        public IPacketReceiver Receiver => _peerReceiver;
        public IPacketSender Sender { get; set; }
        public Guid Identifier { get; set; } = Guid.NewGuid();

        public void Dispose()
        {
            _peer.Disconnect();
        }

        public void QueueData(NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            _peerReceiver.QueueData(reader, channel);
        }
    }
}