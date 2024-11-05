using EchoSync.Transport;
using LiteNetLib;

namespace LiteNetLibAdapters.Senders;

public class ClientSender(NetManager client) : IPacketSender
{
    public void SendPacket(int channel, Reliability reliability, ReadOnlySpan<byte> data)
    {
        switch (reliability)
        {
            case Reliability.Unreliable:
                client.SendToAll(data, DeliveryMethod.Unreliable);
                return;
            case Reliability.Sequenced:
                client.SendToAll(data, DeliveryMethod.Sequenced);
                return;
            case Reliability.Reliable:
                client.SendToAll(data, DeliveryMethod.ReliableOrdered);
                return;
        }
    }
}