using EchoSync.Transport;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LiteNetLibAdapters.Senders
{
    public class ClientSender : IPacketSender
    {
        private readonly NetManager _client;

        public ClientSender(NetManager client)
        {
            _client = client;
        }

        public void SendPacket(int channel, Reliability reliability, ReadOnlySpan<byte> data)
        {
            NetDataWriter writer = NetDataWriter.FromBytes(data.ToArray(), true);
            switch (reliability)
            {
                case Reliability.Unreliable:
                    _client.SendToAll(writer, (byte)channel, DeliveryMethod.Unreliable);
                    return;
                case Reliability.Sequenced:
                    _client.SendToAll(writer, (byte)channel, DeliveryMethod.Sequenced);
                    return;
                case Reliability.Reliable:
                    _client.SendToAll(writer, (byte)channel, DeliveryMethod.ReliableOrdered);
                    return;
            }
        }
    }
}