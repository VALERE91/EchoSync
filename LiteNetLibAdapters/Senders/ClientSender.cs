using EchoSync.Transport;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LiteNetLibAdapters.Senders
{
    public class ClientSender(NetManager client) : IPacketSender
    {
        public void SendPacket(int channel, Reliability reliability, ReadOnlySpan<byte> data)
        {
            NetDataWriter writer = NetDataWriter.FromBytes(data.ToArray(), true);
            switch (reliability)
            {
                case Reliability.Unreliable:
                    client.SendToAll(writer, (byte)channel, DeliveryMethod.Unreliable);
                    return;
                case Reliability.Sequenced:
                    client.SendToAll(writer, (byte)channel, DeliveryMethod.Sequenced);
                    return;
                case Reliability.Reliable:
                    client.SendToAll(writer, (byte)channel, DeliveryMethod.ReliableOrdered);
                    return;
            }
        }
    }
}