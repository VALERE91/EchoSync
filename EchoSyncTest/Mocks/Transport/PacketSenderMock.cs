using EchoSync.Transport;

namespace EchoSyncTest.Mocks.Transport;

public class PacketSenderMock : IPacketSender
{
    public void SendPacket(int channel, Reliability reliability, ReadOnlySpan<byte> data)
    {
        
    }
}