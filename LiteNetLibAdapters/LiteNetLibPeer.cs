using EchoSync.Transport;
using LiteNetLib;
using LiteNetLibAdapters.Receivers;
using LiteNetLibAdapters.Senders;

namespace LiteNetLibAdapters;

public class LiteNetLibPeer(NetPeer peer) : IPeer
{
    private readonly PeerReceiver _peerReceiver = new PeerReceiver();
    public IPacketReceiver Receiver => _peerReceiver;
    public IPacketSender Sender { get; set; } = new PeerSender(peer);
    public Guid Identifier { get; set; } = Guid.NewGuid();

    public void Dispose()
    {
        peer.Disconnect();
    }

    public void QueueData(NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        _peerReceiver.QueueData(reader, channel);
    }
}