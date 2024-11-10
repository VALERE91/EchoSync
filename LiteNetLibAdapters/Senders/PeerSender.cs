using EchoSync.Transport;
using LiteNetLib;

namespace LiteNetLibAdapters.Senders;

public class PeerSender(NetPeer peer) : IPacketSender
{
    public void SendPacket(int channel, Reliability reliability, ReadOnlySpan<byte> data)
    {
        switch (reliability)
        {
            case Reliability.Unreliable:
                peer.Send(data, DeliveryMethod.Unreliable);
                return;
            case Reliability.Sequenced:
                peer.Send(data, DeliveryMethod.Sequenced);
                return;
            case Reliability.Reliable:
                peer.Send(data, DeliveryMethod.ReliableOrdered);
                return;
        }
    }
}