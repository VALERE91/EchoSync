using System;
using EchoSync.Transport;
using LiteNetLib;

namespace LiteNetLibAdapters.Senders
{
    public class PeerSender : IPacketSender
    {
        private readonly NetPeer _peer;

        public PeerSender(NetPeer peer)
        {
            _peer = peer;
        }

        public void SendPacket(int channel, Reliability reliability, ReadOnlySpan<byte> data)
        {
            switch (reliability)
            {
                case Reliability.Unreliable:
                    _peer.Send(data, DeliveryMethod.Unreliable);
                    return;
                case Reliability.Sequenced:
                    _peer.Send(data, DeliveryMethod.Sequenced);
                    return;
                case Reliability.Reliable:
                    _peer.Send(data, DeliveryMethod.ReliableOrdered);
                    return;
            }
        }
    }
}