using EchoSync.Transport;
using EchoSyncTest.Mocks.Transport;

namespace EchoSyncTest.Mocks;

public class ClientMock : IClient
{
    public IPacketReceiver Receiver { get; set; } = new PacketReceiverMock();

    public IPacketSender Sender { get; set; } = new PacketSenderMock();

    public Guid Identifier { get; set; } = Guid.NewGuid();
    public void Connect()
    {
        
    }

    public void Dispose()
    {
        
    }

    public void Tick(float deltaTimeSeconds)
    {
        
    }
}